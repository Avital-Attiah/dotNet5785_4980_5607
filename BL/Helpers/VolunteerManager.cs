using DalApi;
using System.Text;
using System.Text.RegularExpressions;
using System;
using System.Security.Cryptography;
namespace Helpers;

internal static class VolunteerManager
{
    private static IDal s_dal = Factory.Get;

    public static bool IsValidEmail(string email)
    {
        var emailRegex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
        return emailRegex.IsMatch(email);
    }

    public static bool IsValidPhoneNumber(string phoneNumber)
    {
        return long.TryParse(phoneNumber, out _);
    }

    public static bool IsValidLength(string input, int minLength, int maxLength)
    {
        return input.Length >= minLength && input.Length <= maxLength;
    }

    public static bool IsValidId(int id)
    {
        if (id < 100000000 || id > 999999999) return false;

        int sum = 0;
        bool isOddPosition = false;

        for (int i = 0; i < 8; i++)
        {
            int digit = id % 10;
            id /= 10;

            if (isOddPosition)
            {
                int doubled = digit * 2;
                sum += doubled > 9 ? doubled - 9 : doubled;
            }
            else
            {
                sum += digit;
            }

            isOddPosition = !isOddPosition;
        }

        int checkDigit = id % 10;
        return (sum % 10 == checkDigit);
    }

    public static bool IsStrongPassword(string password)
    {
        if (password.Length < 8)
            return false;
        if (!Regex.IsMatch(password, "[A-Z]"))
            return false;
        if (!Regex.IsMatch(password, "[a-z]"))
            return false;
        if (!Regex.IsMatch(password, "[0-9]"))
            return false;
        if (!Regex.IsMatch(password, "[^a-zA-Z0-9]"))
            return false;

        return true;
    }

    public static string HashPassword(string password)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder sb = new StringBuilder();
            foreach (byte b in bytes)
                sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
    }

    public static Tuple<int, int, int, int> GetTotalsCalls(IEnumerable<DO.Assignment> doAssignments)
    {
        int totalHandledCalls = doAssignments.Count(a => a.Status == DO.Enums.TreatmentStatus.CompletedOnTime);
        int totalCancelledVCalls = doAssignments.Count(a => a.Status == DO.Enums.TreatmentStatus.CanceledByVolunteer);
        int totalCancelledMCalls = doAssignments.Count(a => a.Status == DO.Enums.TreatmentStatus.CanceledByVolunteer);
        int totalExpiredSelectedCalls = doAssignments.Count(a => a.Status == DO.Enums.TreatmentStatus.Expired);
        return new Tuple<int, int, int, int>(totalHandledCalls, totalCancelledVCalls, totalCancelledMCalls, totalExpiredSelectedCalls);
    }
    public static bool IsValidEnum<T>(int value) where T : Enum
    {
        return Enum.IsDefined(typeof(T), value);
    }

    public static void ValidateVolunteer(BO.Volunteer boVolunteer, bool isUpdate = false)
    {
        // validation
        if (!IsValidId(boVolunteer.Id))
            throw new BO.BlValidationException($"Invalid ID: {boVolunteer.Id}.");

        if (!IsValidLength(boVolunteer.FullName, 2, 50))
            throw new BO.BlValidationException($"Invalid name length: {boVolunteer.FullName}.");

        if (!IsValidPhoneNumber(boVolunteer.Phone))
            throw new BO.BlValidationException($"Invalid phone number: {boVolunteer.Phone}.");

        if (!IsValidEmail(boVolunteer.Email))
            throw new BO.BlValidationException($"Invalid email address: {boVolunteer.Email}.");

        if (!isUpdate && string.IsNullOrWhiteSpace(boVolunteer.Password))
            throw new BO.BlNullPropertyException("A password must have a value.");

        if (!string.IsNullOrEmpty(boVolunteer.Password))
        {
            if (!IsStrongPassword(boVolunteer.Password))
                throw new BO.BlValidationException("The provided password is not strong enough.");

            boVolunteer.Password = HashPassword(boVolunteer.Password);
        }

        if (string.IsNullOrEmpty(boVolunteer.Address))
            throw new BO.BlNullPropertyException("Address cannot be empty.");

        try
        {
            var (latitude, longitude) = CallManager.GetCoordinates(boVolunteer.Address);
            boVolunteer.Latitude = latitude;
            boVolunteer.Longitude = longitude;
        }
        catch (BO.BlValidationException ex)
        {
            throw;
        }
    }

    public static DO.Volunteer updateDoVolunteerIfNeeded(DO.Volunteer doVolunteer, BO.Volunteer boVolunteer)
    {
        var copyDoVolunteer = doVolunteer;
        if (doVolunteer.FullName != boVolunteer.FullName)
        {
            // Validate name length
            copyDoVolunteer = copyDoVolunteer with { FullName = boVolunteer.FullName };
        }

        // Validate and update password
        if (boVolunteer.Password != null && doVolunteer.Password != boVolunteer.Password)
        {
            var EncryptedPassword = HashPassword(boVolunteer.Password);
            copyDoVolunteer = copyDoVolunteer with { Password = EncryptedPassword };
        }

        // Validate and update phone number
        if (doVolunteer.Phone != boVolunteer.Phone)
        {
            copyDoVolunteer = copyDoVolunteer with { Phone = boVolunteer.Phone };
        }

        // Validate and update email
        if (doVolunteer.Email != boVolunteer.Email)
        {
            copyDoVolunteer = copyDoVolunteer with { Email = boVolunteer.Email };
        }

        // Validate and update address
        if (boVolunteer.Address != null && doVolunteer.CurrentAddress != boVolunteer.Address)
        {
            try
            {
                var (latitude, longitude) = CallManager.GetCoordinates(boVolunteer.Address);
                copyDoVolunteer = copyDoVolunteer with { CurrentAddress = boVolunteer.Address, Latitude = latitude, Longitude = longitude };
            }
            catch (BO.BlValidationException)
            {
                throw;
            }
        }
        // Update role (admin only)
        if ((BO.Role)doVolunteer.Role != boVolunteer.Role)
        {
            if (doVolunteer.Role != DO.Enums.Role.Manager)
            {
                throw new BO.BlUnauthorizedAccessException("Only a manager can update the volunteer's role.");
            }
            copyDoVolunteer = copyDoVolunteer with { Role = (DO.Enums.Role)boVolunteer.Role };
        }

        // Update active status
        if (doVolunteer.IsActive != boVolunteer.IsActive)
        {
            copyDoVolunteer = copyDoVolunteer with { IsActive = boVolunteer.IsActive };
        }

        // Update distance
        if (doVolunteer.MaxDistance != boVolunteer.MaxCallDistance)
        {
            if (boVolunteer.MaxCallDistance < 0)
            {
                throw new BO.BlValidationException("Distance cannot be negative.");
            }
            copyDoVolunteer = copyDoVolunteer with { MaxDistance = boVolunteer.MaxCallDistance };
        }

        // Update distance type
        if (IsValidEnum<BO.DistanceType>((int)boVolunteer.DistanceType))
        {
            if (doVolunteer.DistanceType != (DO.Enums.DistanceType)boVolunteer.DistanceType)
            {
                copyDoVolunteer = copyDoVolunteer with { DistanceType = (DO.Enums.DistanceType)boVolunteer.DistanceType };
            }
        }
        else
        {
            throw new BO.BlValidationException($"Invalid TypeDistance value: {boVolunteer.DistanceType}");
        }
        return copyDoVolunteer;
    }

}