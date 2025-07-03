using DalApi;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
namespace Helpers;

internal static class VolunteerManager
{
    internal static ObserverManager Observers = new(); //stage 5 
    private static IDal s_dal = Factory.Get;
     private static readonly Random s_rand = new(); //stage 7
    private static int s_simulatorCounter = 0; //stage 7

    internal static void SimulateVolunteerActivity()
    {
        Thread.CurrentThread.Name = $"Simulator{++s_simulatorCounter}";
        LinkedList<int> volunteersToNotify = new();

        List<DO.Volunteer> activeVolunteers;
        lock (AdminManager.BlMutex)
            activeVolunteers = s_dal.Volunteer.ReadAll(v => v.IsActive).ToList();

        foreach (var volunteer in activeVolunteers)
        {
            int volunteerId = volunteer.Id;
            DO.Assignment? currentAssignment;

            lock (AdminManager.BlMutex)
                currentAssignment = s_dal.Assignment.Read(a => a.VolunteerId == volunteerId && a.CompletionTime == null);

            if (currentAssignment == null)
            {
                if (s_rand.Next(100) < 20) // 20% הסתברות לטפל
                {
                    var openCalls = CallManager.GetOpenCallsForVolunteer(volunteerId).ToList();
                    if (openCalls.Count > 0)
                    {
                        var selected = openCalls[s_rand.Next(openCalls.Count)];
                        lock (AdminManager.BlMutex)
                        {
                            var newAssignment = new DO.Assignment(0, selected.CallId, volunteerId, AdminManager.Now, null, DO.Enums.TreatmentStatus.InProgress);
                            s_dal.Assignment.Create(newAssignment);
                        }
                        volunteersToNotify.AddLast(volunteerId);
                    }
                }
            }
            else
            {
                TimeSpan elapsed = AdminManager.Now - currentAssignment.EntryTime;
                double distance = 0;
                lock (AdminManager.BlMutex)
                {
                    var call = s_dal.Call.Read(currentAssignment.CallId);
                    if (call?.Latitude != null && call.Longitude != null && volunteer.Latitude != null && volunteer.Longitude != null)
                    {
                        distance = CallManager.CalculateHaversineDistance(volunteer.Latitude, volunteer.Longitude, call.Latitude ?? 0, call.Longitude ?? 0);
                    }
                }

                TimeSpan threshold = TimeSpan.FromMinutes(distance * 1.2 + s_rand.Next(1, 6));

                if (elapsed > threshold)
                {
                    lock (AdminManager.BlMutex)
                    {
                        var updated = currentAssignment with { CompletionTime = AdminManager.Now, Status = DO.Enums.TreatmentStatus.CompletedOnTime };
                        s_dal.Assignment.Update(updated);
                    }
                    volunteersToNotify.AddLast(volunteerId);
                }
                else if (s_rand.Next(100) < 10) // 10% לביטול טיפול
                {
                    lock (AdminManager.BlMutex)
                    {
                        var canceled = currentAssignment with { CompletionTime = AdminManager.Now, Status = DO.Enums.TreatmentStatus.CanceledByVolunteer };
                        s_dal.Assignment.Update(canceled);
                    }
                    volunteersToNotify.AddLast(volunteerId);
                }
            }
        }

        foreach (var id in volunteersToNotify)
            Observers.NotifyItemUpdated(id); // stage 5+7

        // ✅ עדכוני UI
        CallManager.Observers.NotifyListUpdated();
        VolunteerManager.Observers.NotifyListUpdated();

    }

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
        lock (AdminManager.BlMutex) //stage 7
        {
            int totalHandledCalls = doAssignments.Count(a => a.Status == DO.Enums.TreatmentStatus.CompletedOnTime);
            int totalCancelledVCalls = doAssignments.Count(a => a.Status == DO.Enums.TreatmentStatus.CanceledByVolunteer || a.Status == DO.Enums.TreatmentStatus.CanceledByManager);
            int totalExpiredSelectedCalls = doAssignments.Count(a => a.Status == DO.Enums.TreatmentStatus.Expired);
            return new Tuple<int, int, int>(totalHandledCalls, totalCancelledVCalls, totalExpiredSelectedCalls);
        }
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
    public static DO.Volunteer updateDoVolunteerIfNeeded(DO.Volunteer doVolunteer, BO.Volunteer boVolunteer, bool skipCoordinates = false)
    {
        AdminManager.ThrowOnSimulatorIsRunning();

        var copyDoVolunteer = doVolunteer;

        if (doVolunteer.FullName != boVolunteer.FullName)
            copyDoVolunteer = copyDoVolunteer with { FullName = boVolunteer.FullName };

        if (boVolunteer.Password != null && doVolunteer.Password != HashPassword(boVolunteer.Password))
        {
            var encryptedPassword = HashPassword(boVolunteer.Password);
            copyDoVolunteer = copyDoVolunteer with { Password = encryptedPassword };
        }

        if (doVolunteer.Phone != boVolunteer.Phone)
            copyDoVolunteer = copyDoVolunteer with { Phone = boVolunteer.Phone };

        if (doVolunteer.Email != boVolunteer.Email)
            copyDoVolunteer = copyDoVolunteer with { Email = boVolunteer.Email };

        // Handle address & coordinates update based on skipCoordinates
        if (!skipCoordinates && boVolunteer.Address != null && doVolunteer.CurrentAddress != boVolunteer.Address)
        {
            try
            {
                var (latitude, longitude) = CallManager.GetCoordinates(boVolunteer.Address);
                copyDoVolunteer = copyDoVolunteer with
                {
                    CurrentAddress = boVolunteer.Address,
                    Latitude = latitude,
                    Longitude = longitude
                };
            }
            catch (BO.BlValidationException) { throw; }
        }
        else if (skipCoordinates && boVolunteer.Address != null && doVolunteer.CurrentAddress != boVolunteer.Address)
        {
            // Update only the address – coordinates will be updated async
            copyDoVolunteer = copyDoVolunteer with { CurrentAddress = boVolunteer.Address };
        }

        if ((BO.Role)doVolunteer.Role != boVolunteer.Role)
        {
            if (doVolunteer.Role != DO.Enums.Role.Manager)
                throw new BO.BlUnauthorizedAccessException("Only a manager can update the volunteer's role.");

            copyDoVolunteer = copyDoVolunteer with { Role = (DO.Enums.Role)boVolunteer.Role };
        }

        if (doVolunteer.IsActive != boVolunteer.IsActive)
            copyDoVolunteer = copyDoVolunteer with { IsActive = boVolunteer.IsActive };

        if (doVolunteer.MaxDistance != boVolunteer.MaxCallDistance)
        {
            if (boVolunteer.MaxCallDistance < 0)
                throw new BO.BlValidationException("Distance cannot be negative.");
            copyDoVolunteer = copyDoVolunteer with { MaxDistance = boVolunteer.MaxCallDistance };
        }

        if (IsValidEnum<BO.DistanceType>((int)boVolunteer.DistanceType))
        {
            if (doVolunteer.DistanceType != (DO.Enums.DistanceType)boVolunteer.DistanceType)
            {
                copyDoVolunteer = copyDoVolunteer with
                {
                    DistanceType = (DO.Enums.DistanceType)boVolunteer.DistanceType
                };
            }
        }
        else
        {
            throw new BO.BlValidationException($"Invalid DistanceType value: {boVolunteer.DistanceType}");
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

    private static async Task updateCoordinatesForVolunteerAddressAsync(DO.Volunteer doVolunteer)
    {
        if (doVolunteer.CurrentAddress is not null)
        {
            Tools.Location? loc = await Tools.GetLocationOfAddressAsync(doVolunteer.CurrentAddress);
            if (loc is not null)
            {
                doVolunteer = doVolunteer with { Latitude = loc.Latitude, Longitude = loc.Longitude };
                lock (AdminManager.BlMutex)
                    s_dal.Volunteer.Update(doVolunteer);

                Observers.NotifyListUpdated();    // עדכון רשימה
                Observers.NotifyItemUpdated(doVolunteer.Id); // עדכון פריט בודד
            }
        }
    }

    internal static DO.Volunteer updateDoVolunteerIfNeededWithoutCoordinates(DO.Volunteer doVolunteer, BO.Volunteer boVolunteer)
    {
        AdminManager.ThrowOnSimulatorIsRunning();

        var copy = doVolunteer;

        if (doVolunteer.FullName != boVolunteer.FullName)
            copy = copy with { FullName = boVolunteer.FullName };

        if (boVolunteer.Password != null && doVolunteer.Password != HashPassword(boVolunteer.Password))
            copy = copy with { Password = HashPassword(boVolunteer.Password) };

        if (doVolunteer.Phone != boVolunteer.Phone)
            copy = copy with { Phone = boVolunteer.Phone };

        if (doVolunteer.Email != boVolunteer.Email)
            copy = copy with { Email = boVolunteer.Email };

        if (doVolunteer.CurrentAddress != boVolunteer.Address)
            copy = copy with { CurrentAddress = boVolunteer.Address };

        if ((BO.Role)doVolunteer.Role != boVolunteer.Role)
        {
            if (doVolunteer.Role != DO.Enums.Role.Manager)
                throw new BO.BlUnauthorizedAccessException("Only a manager can update the volunteer's role.");
            copy = copy with { Role = (DO.Enums.Role)boVolunteer.Role };
        }

        if (doVolunteer.IsActive != boVolunteer.IsActive)
            copy = copy with { IsActive = boVolunteer.IsActive };

        if (doVolunteer.MaxDistance != boVolunteer.MaxCallDistance)
        {
            if (boVolunteer.MaxCallDistance < 0)
                throw new BO.BlValidationException("Distance cannot be negative.");
            copy = copy with { MaxDistance = boVolunteer.MaxCallDistance };
        }

        if (IsValidEnum<BO.DistanceType>((int)boVolunteer.DistanceType)
            && doVolunteer.DistanceType != (DO.Enums.DistanceType)boVolunteer.DistanceType)
        {
            copy = copy with { DistanceType = (DO.Enums.DistanceType)boVolunteer.DistanceType };
        }

        return copy;
    }
    internal static async Task UpdateCoordinatesForVolunteerAsync(DO.Volunteer doVolunteer)
    {
        if (!string.IsNullOrEmpty(doVolunteer.CurrentAddress))
        {
            var loc = await Tools.GetLocationOfAddressAsync(doVolunteer.CurrentAddress);
            if (loc is not null)
            {
                var updated = doVolunteer with { Latitude = loc.Latitude, Longitude = loc.Longitude };
                lock (AdminManager.BlMutex)
                    s_dal.Volunteer.Update(updated);

                Observers.NotifyItemUpdated(updated.Id);
                Observers.NotifyListUpdated();
            }
        }


    }

    


}
