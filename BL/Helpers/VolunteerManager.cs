using DalApi;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
namespace Helpers;

internal static class VolunteerManager
{
    internal static ObserverManager Observers = new(); //stage 5 
    private static IDal s_dal = Factory.Get;

    /// <summary>
    /// Validates if the provided email is in the correct format.
    /// </summary>
    public static bool IsValidEmail(string email)
    {
        var emailRegex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
        return emailRegex.IsMatch(email);
    }

    /// <summary>
    /// Validates if the provided phone number is numeric.
    /// </summary>
    public static bool IsValidPhoneNumber(string phoneNumber)
    {
        return long.TryParse(phoneNumber, out _);
    }

    /// <summary>
    /// Validates if the input string is within the specified length range.
    /// </summary>
    public static bool IsValidLength(string input, int minLength, int maxLength)
    {
        return input.Length >= minLength && input.Length <= maxLength;
    }

    /// <summary>
    /// Validates if the provided ID is a valid ID according to a specific checksum algorithm.
    /// </summary>
    public static bool IsValidId(int id)
    {
        string idStr = id.ToString().PadLeft(9, '0'); // משלימים ל-9 ספרות במקרה שחסרות

        int sum = 0;
        bool isOddPosition = true;  // מתחילים מהספרה הימנית ביותר

        for (int i = 8; i >= 0; i--)  // לולאה מימין לשמאל
        {
            int digit = idStr[i] - '0';  // המרה למספר

            if (!isOddPosition)  // כל ספרה שנייה מוכפלת ב-2
            {
                int doubled = digit * 2;
                sum += (doubled > 9) ? (doubled - 9) : doubled;
            }
            else
            {
                sum += digit;
            }

            isOddPosition = !isOddPosition;
        }

        return (sum % 10 == 0);
    }

    /// <summary>
    /// Validates if the password is strong based on specific criteria (length, uppercase, lowercase, number, special character).
    /// </summary>
    public static bool IsStrongPassword(string password)
    {
        if (password.Length < 8)
            return false;
        if (!Regex.IsMatch(password, "[a-z]"))
            return false;
        if (!Regex.IsMatch(password, "[0-9]"))
            return false;
        return true;
    }

    /// <summary>
    /// Hashes the password using SHA-256.
    /// </summary>
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

    /// <summary>
    /// Returns the totals for handled calls, cancelled calls by volunteer/manager, and expired selected calls.
    /// </summary>
    public static Tuple<int, int, int> GetTotalsCalls(IEnumerable<DO.Assignment> doAssignments)
    {
        int totalHandledCalls = doAssignments.Count(a => a.Status == DO.Enums.TreatmentStatus.CompletedOnTime);
        int totalCancelledVCalls = doAssignments.Count(a => a.Status == DO.Enums.TreatmentStatus.CanceledByVolunteer || a.Status == DO.Enums.TreatmentStatus.CanceledByManager);
        int totalExpiredSelectedCalls = doAssignments.Count(a => a.Status == DO.Enums.TreatmentStatus.Expired);
        return new Tuple<int, int, int>(totalHandledCalls, totalCancelledVCalls, totalExpiredSelectedCalls);
    }

    /// <summary>
    /// Validates if the given value is a valid enum value of the specified type.
    /// </summary>
    public static bool IsValidEnum<T>(int value) where T : Enum
    {
        return Enum.IsDefined(typeof(T), value);
    }

    /// <summary>
    /// Validates the volunteer properties and ensures that required fields are correctly set.
    /// </summary>
    public static void ValidateVolunteer(BO.Volunteer boVolunteer, bool isUpdate = false)
    {
        // validation
        if (!isUpdate)
        {
            if (!IsValidId(boVolunteer.Id))
                throw new BO.BlValidationException($"Invalid ID: {boVolunteer.Id}.");
        }
           
        if (!IsValidLength(boVolunteer.FullName, 2, 50))
            throw new BO.BlValidationException($"Invalid name length: {boVolunteer.FullName}.");

        if (!IsValidPhoneNumber(boVolunteer.Phone))
            throw new BO.BlValidationException($"Invalid phone number: {boVolunteer.Phone}.");

        if (!IsValidEmail(boVolunteer.Email))
            throw new BO.BlValidationException($"Invalid email address: {boVolunteer.Email}.");

        if (string.IsNullOrWhiteSpace(boVolunteer.Password))
            throw new BO.BlNullPropertyException("A password must have a value.");

        if (!string.IsNullOrEmpty(boVolunteer.Password))
        {
            if (!IsStrongPassword(boVolunteer.Password))
            throw new BO.BlValidationException("The provided password is not strong enough.");
        }
        

        if (string.IsNullOrEmpty(boVolunteer.Address))
            return;
    }

    /// <summary>
    /// Updates a DO (Data Object) volunteer if needed by comparing with BO volunteer data.
    /// </summary>
    public static DO.Volunteer updateDoVolunteerIfNeeded(DO.Volunteer doVolunteer, BO.Volunteer boVolunteer)
    {
        var copyDoVolunteer = doVolunteer;
        if (doVolunteer.FullName != boVolunteer.FullName)
        {
            copyDoVolunteer = copyDoVolunteer with { FullName = boVolunteer.FullName };
        }

        // Validate and update password
        if (boVolunteer.Password != null && doVolunteer.Password != HashPassword(boVolunteer.Password))
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
    private static readonly Random random = new Random();

    public static string GeneratePassword()
    {
        int length = 20;
        const string lowerCase = "abcdefghijklmnopqrstuvwxyz";
        const string upperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string digits = "0123456789";
        string allChars = lowerCase + upperCase + digits;

        string password = new string(allChars.OrderBy(_ => random.Next()).Take(length).ToArray());

        return password;
    }

}
