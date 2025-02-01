using System.Security.Principal;



namespace BlTest
{
    internal class Program
    {

        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        static void Main(string[] args)
        {
            try
            {
                bool exit = false;

                while (!exit)
                {

                    Console.WriteLine("=== Main Menu ===");
                    Console.WriteLine("1. Volunteer Management");
                    Console.WriteLine("2. Call Management");
                    Console.WriteLine("3. Admin Management");
                    Console.WriteLine("4. Exit");
                    Console.Write("Enter your choice: ");

                    string choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            ManageVolunteers();
                            break;
                        case "2":
                            ManageCalls();
                            break;
                        case "3":
                            ManageAdmin();
                            break;
                        case "4":
                            exit = true;
                            Console.WriteLine("Exiting the program...");
                            break;
                        default:
                            Console.WriteLine("Invalid choice, please try again.");
                            break;
                    }

                    if (!exit)
                    {
                        Console.WriteLine("\nPress any key to continue...");
                        Console.ReadKey();
                    }
                }
            }
         
            catch (Exception ex) // חריגות כלליות
            {
                Console.WriteLine("An unexpected error occurred:");
                Console.WriteLine($"Exception Type: {ex.GetType().Name}");
                Console.WriteLine($"Message: {ex.Message}");

                if (ex.InnerException != null)
                {
                    Console.WriteLine("Inner Exception:");
                    Console.WriteLine($"Type: {ex.InnerException.GetType().Name}");
                    Console.WriteLine($"Message: {ex.InnerException.Message}");
                }
            }
        }

        static void ManageVolunteers()
        {
            bool back = false;
            while (!back)
            {
                Console.WriteLine("=== Volunteer Management ===");
                Console.WriteLine("1. Login");
                Console.WriteLine("2. Get Volunteers List");
                Console.WriteLine("3. Read Volunteer");
                Console.WriteLine("4. Update Volunteer Details");
                Console.WriteLine("5. Delete Volunteer");
                Console.WriteLine("6. Add Volunteer");
                Console.WriteLine("7. Back to Main Menu");
                Console.Write("Enter your choice: ");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        Console.WriteLine("Enter volunteer fullname:");
                        string fullname = Console.ReadLine();
                        Console.WriteLine("Enter volunteer password:");
                        string password = Console.ReadLine();
                        BO.Role role = s_bl.Volunteer.Login(fullname, password);
                        Console.WriteLine($"your Role is:{role}");

                        break;
                    case "2":
                        Console.WriteLine("Are you active?");
                        bool isActive = bool.Parse(Console.ReadLine());
                        // שאל את המשתמש על אפשרות המיון
                        Console.WriteLine("Sort by:");
                        Console.WriteLine("1. ID");
                        Console.WriteLine("2. Full Name");
                        Console.WriteLine("3. Is Available");
                        Console.WriteLine("4. Sum Treated Calls");
                        Console.WriteLine("5. Sum Calls Self Cancelled");
                        Console.WriteLine("6. Sum Expired Calls");
                        Console.WriteLine("7. Call ID");
                        Console.WriteLine("8. Call Type");

                        int sortOption = int.Parse(Console.ReadLine());

                        // המרת בחירת המשתמש לסוג המיון המתאים
                        BO.VolunteerInLIstFields? sortField = sortOption switch
                        {
                            1 => BO.VolunteerInLIstFields.Id,
                            2 => BO.VolunteerInLIstFields.FullName,
                            3 => BO.VolunteerInLIstFields.IsActive,
                            4 => BO.VolunteerInLIstFields.TotalHandledCalls,
                            5 => BO.VolunteerInLIstFields.TotalCancelledCalls,
                            6 => BO.VolunteerInLIstFields.TotalExpiredSelectedCalls,
                            7 => BO.VolunteerInLIstFields.CallId,
                            8 => BO.VolunteerInLIstFields.TypeCall,
                            _ => null  // ברירת מחדל אם אין בחירה חוקית
                        };

                        // קריאה לפונקציה עם המיון שבחר המשתמש
                        var volunteersList = s_bl.Volunteer.GetVolunteersList(isActive, sortField);
                        foreach (var volunteer in volunteersList)
                        {
                            Console.WriteLine($"ID: {volunteer.Id}, Name: {volunteer.FullName}, Available: {volunteer.IsActive}");
                        }
                        break;
                    case "3":
                        Console.WriteLine("Enter your Id:");
                        int Id = int.Parse(Console.ReadLine());
                        s_bl.Volunteer.Read(Id);
                        break;
                    case "4":
                        CreateOrUpdate("volunteer", false);
                        break;
                    case "5":
                        DeletObject("volunteer");
                        break;
                    case "6":
                        CreateOrUpdate("volunteer", true);
                        break;
                    case "7":
                        back = true;
                        break;
                    default:
                        Console.WriteLine("Invalid choice, please try again.");
                        break;
                }
                if (!back) Console.ReadKey();
            }
        }

        static void ManageCalls()
        {
            bool back = false;
            int assignmentId;
            int volunteerId;
            int callId;
            while (!back)
            {

                Console.WriteLine("=== Call Management ===");
                Console.WriteLine("1. Get Call Counts By Status");
                Console.WriteLine("2. Get Filtered And Sorted Calls");
                Console.WriteLine("3. Read Call");
                Console.WriteLine("4. Update Call");
                Console.WriteLine("5. Delete Call");
                Console.WriteLine("6. Add Call");
                Console.WriteLine("7. Complete Call Treatment");
                Console.WriteLine("8. Cancel Call Treatment");
                Console.WriteLine("9. Assign Call To Volunteer");
                Console.WriteLine("10. Back to Main Menu");
                Console.Write("Enter your choice: ");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        int[] callCount = s_bl.Call.GetCallCounts();
                        foreach (int i in callCount) Console.WriteLine(i + " ");
                        break;
                    case "2":

                        Console.WriteLine("Filter by:");
                        Console.WriteLine("1. ID");
                        Console.WriteLine("2. CallId");
                        Console.WriteLine("3. Call Type");
                        Console.WriteLine("4. Opening Time");
                        Console.WriteLine("5. Remaining Time");
                        Console.WriteLine("6. Last Volunteer Name");
                        Console.WriteLine("7. Total Handling Time");
                        Console.WriteLine("8. Call Status");
                        Console.WriteLine("9. Total Assignments");

                        int filterOption = int.Parse(Console.ReadLine());
                        BO.CallInListFieldSor? filterField = (BO.CallInListFieldSor?)filterOption;

                        object filterValue = null;
                        if (filterField.HasValue)
                        {
                            Console.Write("Enter filter value: ");
                            filterValue = Console.ReadLine();
                        }

                        Console.WriteLine("Sort by:");
                        Console.WriteLine("1. ID");
                        Console.WriteLine("2. CallId");
                        Console.WriteLine("3. Call Type");
                        Console.WriteLine("4. Opening Time");
                        Console.WriteLine("5. Remaining Time");
                        Console.WriteLine("6. Last Volunteer Name");
                        Console.WriteLine("7. Total Handling Time");
                        Console.WriteLine("8. Call Status");
                        Console.WriteLine("9. Total Assignments");

                        int sortOption = int.Parse(Console.ReadLine());
                        BO.CallInListFieldSor? sortField = (BO.CallInListFieldSor?)sortOption;

                        s_bl.Call.GetCallsList(filterField, filterValue, sortField);
                        break;

                    case "3":
                        Console.WriteLine("insert callId to read");
                        callId = int.Parse(Console.ReadLine());
                        s_bl.Call.Read(callId);
                        break;
                    case "4":
                        CreateOrUpdate("call", false);
                        break;
                    case "5":
                        DeletObject("call");
                        break;
                    case "6":
                        CreateOrUpdate("call", true);
                        break;
                    case "7":
                        Console.WriteLine("Enter volunteerID:");
                        volunteerId = int.Parse(Console.ReadLine());
                        Console.WriteLine("Enter assignmentID");
                        assignmentId = int.Parse(Console.ReadLine());
                        s_bl.Call.FinishCall(volunteerId, assignmentId);
                        break;
                    case "8":
                        Console.WriteLine("Enter callID:");
                        callId = int.Parse(Console.ReadLine());
                        Console.WriteLine("Enter assignmentID");
                        assignmentId = int.Parse(Console.ReadLine());
                        s_bl.Call.CancellationOfTreatment(callId, assignmentId);
                        break;
                    case "9":
                        Console.WriteLine("Enter volunteerID:");
                        volunteerId = int.Parse(Console.ReadLine());
                        Console.WriteLine("Enter volunteerID:");
                        callId = int.Parse(Console.ReadLine());
                        s_bl.Call.SelectCall(volunteerId, callId);
                        break;
                    case "10":
                        back = true;
                        break;
                    default:
                        Console.WriteLine("Invalid choice, please try again.");
                        break;
                }
                if (!back) Console.ReadKey();
            }
        }


        static void ManageAdmin()
        {
            bool back = false;
            while (!back)
            {

                Console.WriteLine("=== Admin Management ===");
                Console.WriteLine("1. Initialize Database");
                Console.WriteLine("2. Reset Database");
                Console.WriteLine("3. Get Clock");
                Console.WriteLine("4. Advance Clock");
                Console.WriteLine("5. Set Risk Time Span");
                Console.WriteLine("6. Get Risk Time Span");
                Console.WriteLine("7. Back to Main Menu");
                Console.Write("Enter your choice: ");

                string choice = Console.ReadLine();
                DateTime clock;
                switch (choice)
                {
                    case "1":
                        s_bl.Admin.InitializeDB();
                        break;
                    case "2":
                        s_bl.Admin.ResetDB();
                        break;
                    case "3":
                        clock = s_bl.Admin.GetClock();
                        Console.WriteLine(clock);
                        break;
                    case "4":
                        // Ask the user to choose the time unit for advancing the clock
                        Console.WriteLine("Select the time unit to advance the clock:");
                        Console.WriteLine("1. Minute");
                        Console.WriteLine("2. Hour");
                        Console.WriteLine("3. Day");
                        Console.WriteLine("4. Month");
                        Console.WriteLine("5. Year");

                        if (!int.TryParse(Console.ReadLine(), out int timeUnitOption) || timeUnitOption < 1 || timeUnitOption > 5)
                        {
                            Console.WriteLine("Invalid option. Please enter a number between 1 and 5.");
                            break;
                        }

                        // Convert the user input into the corresponding TimeUnit enum
                        BO.TimeUnit selectedTimeUnit = (BO.TimeUnit)(timeUnitOption - 1);

                        // Advance the clock using the selected unit and amount
                        s_bl.Admin.UpdateClock(selectedTimeUnit);

                        // Get and display the updated clock
                        clock = s_bl.Admin.GetClock();
                        Console.WriteLine($"Clock advanced by 1 {selectedTimeUnit}. New time: {clock}");
                        break;

                    case "5":
                        Console.WriteLine("Enter new risk time span (format: hh:mm:ss):");

                        if (!TimeSpan.TryParse(Console.ReadLine(), out TimeSpan riskTimeSpan))
                        {
                            Console.WriteLine("Invalid format. Please enter time in hh:mm:ss format.");
                            break;
                        }

                        s_bl.Admin.SetRiskRange(riskTimeSpan);
                        Console.WriteLine($"Risk time span updated to {riskTimeSpan}");
                        break;
                    case "6":
                        TimeSpan t = s_bl.Admin.GetRiskRange();
                        Console.WriteLine(t);
                        break;
                    case "7":
                        back = true;
                        break;
                    default:
                        Console.WriteLine("Invalid choice, please try again.");
                        break;
                }
                if (!back) Console.ReadKey();
            }
        }

        static void CreateOrUpdate(string type, bool isCreate)
        {
            switch (type)
            {
                case "volunteer":
                    // Gather volunteer details from user input
                    Console.WriteLine("Enter ID:");
                    int Idvolunteer = int.Parse(Console.ReadLine());

                    Console.Write("Enter Full Name: ");
                    string fullName = Console.ReadLine();

                    Console.Write("Enter Phone Number: ");
                    string phone = Console.ReadLine();

                    Console.Write("Enter Email: ");
                    string email = Console.ReadLine();

                    Console.Write("Enter Address (optional): ");
                    string? address = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(address)) address = null;

                    Console.Write("Is Available (true/false, default is false): ");
                    bool isAvailable = bool.Parse(Console.ReadLine() ?? "false");

                    Console.Write("Enter Role (default is Role.Volunteer): ");
                    string roleInput = Console.ReadLine();
                    BO.Role role = string.IsNullOrWhiteSpace(roleInput) ? BO.Role.Volunteer : Enum.Parse<BO.Role>(roleInput);

                    Console.Write("Enter Type Distance (default is TypeDistance.AERIAL): ");
                    string typeDistanceInput = Console.ReadLine();
                    BO.DistanceType typeDistance = string.IsNullOrWhiteSpace(typeDistanceInput) ? BO.DistanceType.Air : Enum.Parse<BO.DistanceType>(typeDistanceInput);

                    Console.Write("Enter Max Distance (optional): ");
                    string input = Console.ReadLine();

                    double? maxDistance = string.IsNullOrWhiteSpace(input) ? null : double.Parse(input);

                    Console.Write("Enter your password: ");
                    string password = Console.ReadLine();
                    if (isCreate)
                    {
                        // Create a new volunteer.
                        BO.Volunteer newVolunteer = new BO.Volunteer
                        {
                            Id = Idvolunteer,
                            FullName = fullName,
                            Phone = phone,
                            Email = email,
                            Address = address,
                            IsActive = isAvailable,
                            Role = role,
                            DistanceType = typeDistance,
                            MaxCallDistance = maxDistance,
                            Latitude = 0.0,
                            Longitude = 0.0,
                            Password = password
                        };
                        s_bl.Volunteer!.Create(newVolunteer);
                    }
                    else
                    {
                        // Update existing volunteer details.
                        BO.Volunteer volunteer = s_bl.Volunteer.Read(Idvolunteer);
                        BO.Volunteer updatedVolunteer = new BO.Volunteer
                        {
                            Id = volunteer.Id,
                            FullName = fullName ?? volunteer.FullName,
                            Phone = phone ?? volunteer.Phone,
                            Email = email ?? volunteer.Email,
                            Address = address ?? volunteer.Address,
                            IsActive = isAvailable,
                            Role = role,
                            DistanceType = typeDistance,
                            MaxCallDistance = maxDistance ?? volunteer.MaxCallDistance,
                            Latitude = volunteer.Latitude,
                            Longitude = volunteer.Longitude,
                            Password = password ?? volunteer.Password
                        };

                        // עדכון הוולונטר עם המידע החדש
                        s_bl.Volunteer!.Update(Idvolunteer, updatedVolunteer);
                    }

                    break;

                case "call":
                    // Gather call details from user input
                    Console.Write("Enter Call Id: ");
                    int idCall = int.Parse(Console.ReadLine());

                    Console.Write("Enter Call Type (1 for Emergency, 2 for Regular): ");
                    BO.CallType callType = (BO.CallType)Enum.Parse(typeof(BO.CallType), Console.ReadLine());

                    Console.Write("Enter Address: ");
                    string addressCall = Console.ReadLine();

                    Console.Write("Enter Open Date (yyyy-MM-dd): ");
                    DateTime openDate = DateTime.Parse(Console.ReadLine());

                    Console.Write("Enter Description (optional): ");
                    string description = Console.ReadLine();

                    Console.Write("Enter Max Time to Finish (yyyy-MM-dd, optional): ");
                    DateTime? maxTimeFinish = string.IsNullOrWhiteSpace(Console.ReadLine()) ? null : DateTime.Parse(Console.ReadLine());

                    if (isCreate)
                    {
                        // Create a new call
                        BO.Call newCall = new BO.Call
                        {
                            Id = idCall,
                            CallType = callType,
                            FullAddress = addressCall,
                            Latitude = 0.0,
                            Longitude = 0.0,
                            OpenTime = openDate,
                            Description = description,
                            MaxCompletionTime = maxTimeFinish,
                            Status = BO.CallStatus.Open
                        };
                        s_bl.Call!.Create(newCall);
                    }
                    else
                    {
                        // Update existing call details
                        BO.Call call = s_bl.Call.Read(idCall);
                        BO.Call updatedCall = new BO.Call
                        {
                            Id = call.Id, // שימור ה-ID הקיים
                            CallType = callType,
                            FullAddress = addressCall ?? call.FullAddress,
                            Latitude = call.Latitude,
                            Longitude = call.Longitude,
                            OpenTime = openDate,
                            Description = description ?? call.Description,
                            MaxCompletionTime = maxTimeFinish ?? call.MaxCompletionTime,
                            Status = call.Status, // שימור הסטטוס הקיים
                            ListAssignments = call.ListAssignments // שימור הרשימה של המשימות הקיימות
                        };

                        // עדכון השיחה ב-DB
                        s_bl.Call!.UpdateCallDetails(updatedCall);
                    }
                    break;
            }
        }

        static void DeletObject(string entityType)
        {
            Console.WriteLine("enter ID:\n");
            int Id = int.Parse(Console.ReadLine());
            switch (entityType)
            {
                case "volunteer":
                    s_bl.Volunteer!.Delete(Id);
                    break;
                case "call":
                    s_bl.Call!.DeleteCall(Id);
                    break;
                default:
                    Console.WriteLine("NO SUCH ENTITY");
                    break;
            }
        }

    }
}