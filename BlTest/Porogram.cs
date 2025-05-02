using BO;
using DalApi;
using DO;
using Microsoft.VisualBasic;
using System.Net;
using System.Numerics;
using System.Security.Principal;
using static DO.Enums;

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
                        var volunteerRead = s_bl.Volunteer.Read(Id);

                        // עכשיו נדפיס את הפרטים:
                        Console.WriteLine($"ID: {volunteerRead.Id}");
                        Console.WriteLine($"Name: {volunteerRead.FullName}");
                        Console.WriteLine($"Phone: {volunteerRead.Phone}");
                        Console.WriteLine($"Email: {volunteerRead.Email}");
                        Console.WriteLine($"Is Active: {volunteerRead.IsActive}");
                        Console.WriteLine($"Total Completed Calls: {volunteerRead.TotalCompletedCalls}");
                        Console.WriteLine($"Current Call Status: {volunteerRead.CurrentCall}");

                        break;
                    case "4":
                        UpdateEninty("volunteer");
                        break;
                    case "5":
                        DeletObject("volunteer");
                        break;
                    case "6":
                        CreateEninty("volunteer");
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
                Console.WriteLine("10. get open calls in list");
                Console.WriteLine("11.get closed calls in list");
                Console.WriteLine("12. Back to Main Menu");
                Console.Write("Enter your choice: ");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        CallStatus[] statuses = (CallStatus[])Enum.GetValues(typeof(CallStatus));
                        int[] callCount = s_bl.Call.GetCallCounts();
                        for (int i = 0; i < callCount.Length; i++)
                        {
                            Console.WriteLine($"{statuses[i]}: {callCount[i]}");
                        }
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
                        Console.WriteLine("9. nothing");

                        BO.CallInListFieldSor? filterField = null;
                        object filterValue = null;

                        if (int.TryParse(Console.ReadLine(), out int filterOption))
                        {
                            filterField = (BO.CallInListFieldSor?)(filterOption-1);

                            if (filterField.HasValue && filterOption != 9) // אם לא בחר "nothing"
                            {
                                Console.Write("Enter filter value: ");
                                string inputValue = Console.ReadLine();

                                // המרת הערך בהתאם לסוג השדה
                                switch (filterField)
                                {
                                    case BO.CallInListFieldSor.Id:
                                    case BO.CallInListFieldSor.CallId:
                                        if (int.TryParse(inputValue, out int intValue))
                                            filterValue = intValue;
                                        break;

                                    case BO.CallInListFieldSor.CallType:
                                        if (Enum.TryParse<BO.CallType>(inputValue, true, out BO.CallType enumValue))  // הוספתי 'true' כדי להתעלם מהתאמה בין רגישות לאותיות
                                            filterValue = enumValue;
                                        else
                                            // אם לא הצלחנו להמיר, ניתן להוסיף טיפול בשגיאה
                                            filterValue = BO.CallType.None;  // לדוגמה, להציב את הערך None אם לא נמצא התאמה
                                        break;

                                    case BO.CallInListFieldSor.TimeOpen:
                                        // המרה ל-DateTime אם יש צורך
                                        if (DateTime.TryParse(inputValue, out DateTime dateValue))
                                        {
                                            filterValue = dateValue;
                                        }
                                        break;

                                    case BO.CallInListFieldSor.RemainingTime:
                                        // המרה ל-TimeSpan אם יש צורך
                                        if (TimeSpan.TryParse(inputValue, out TimeSpan timeValue))
                                        {
                                            filterValue = timeValue;
                                        }
                                        break;

                                    case BO.CallInListFieldSor.LastVolunteerName:
                                        
                                        filterValue = inputValue;
                                        break;

                                    case BO.CallInListFieldSor.Status:
                                        // המרת הקלט לערך המתאים ב־CallStatus (אם הקלט תקין)
                                        if (Enum.TryParse<BO.CallStatus>(inputValue, true, out BO.CallStatus statusValue))
                                        {
                                            filterValue = statusValue;  // שומר את הערך המומר
                                        }
                                        else
                                        {
                                            Console.WriteLine("Invalid status value. Please enter a valid status.");
                                        }
                                        break;

                                }
                            }
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
                        BO.CallInListFieldSor? sortField = (BO.CallInListFieldSor?)(sortOption-1);

                        var sortedItems = s_bl.Call.GetCallsList(filterField, filterValue, sortField);
                        foreach (var item in sortedItems)
                            Console.WriteLine(item);
                        break;





                    case "3":
                        Console.WriteLine("insert call Id to read");
                        callId = int.Parse(Console.ReadLine());
                        var callRead = s_bl.Call.Read(callId);

                        Console.WriteLine($"ID: {callRead.Id}");
                        Console.WriteLine($"Status: {callRead.Status}");
                        Console.WriteLine($"OpenTime: {callRead.OpenTime}");
                        Console.WriteLine($"CallType: {callRead.CallType}");
                        Console.WriteLine($"MaxCompletionTime: {callRead.MaxCompletionTime}");
                        Console.WriteLine($"Total adress: {callRead.FullAddress}");
                        break;
                    case "4":
                        UpdateEninty("call");
                        break;
                    case "5":
                        DeletObject("call");
                        break;
                    case "6":
                        CreateEninty("call");
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
                        Console.WriteLine("Enter CallID:");
                        callId = int.Parse(Console.ReadLine());
                        s_bl.Call.SelectCall(volunteerId, callId);
                        break;
                    case "10":
                        Console.Write("Enter Volunteer ID: ");
                        if (!int.TryParse(Console.ReadLine(), out volunteerId))
                        {
                            Console.WriteLine("Invalid input. Please enter a valid Volunteer ID.");
                            break;
                        }

                        Console.Write("Enter Call Type (optional, press Enter to skip): ");
                        string callTypeInput = Console.ReadLine();
                        BO.CallType? callType = null;

                        if (!string.IsNullOrWhiteSpace(callTypeInput) && Enum.TryParse(callTypeInput, out BO.CallType parsedCallType))
                        {
                            callType = parsedCallType;
                        }

                        Console.Write("Enter Sorting Field (optional, press Enter to skip): ");
                        string sortByInput = Console.ReadLine();
                        BO.OpenCallInListFields? sortByField = null;

                        if (!string.IsNullOrWhiteSpace(sortByInput) && Enum.TryParse(sortByInput, out BO.OpenCallInListFields parsedSortField))
                        {
                            sortByField = parsedSortField;
                        }

                        // Fetch open calls based on user input
                        var openCalls = s_bl.Call.GetOpenCalls(volunteerId, callType, sortByField);

                        // Display the retrieved open calls
                        Console.WriteLine("\nOpen Calls:");
                        foreach (var call in openCalls)
                        {
                            Console.WriteLine(call);
                        }



                        break;
                    case "11":
                    Console.Write("Enter Volunteer ID: ");
                    if (!int.TryParse(Console.ReadLine(), out volunteerId))
                    {
                        Console.WriteLine("Invalid input. Please enter a valid Volunteer ID.");
                        break;
                    }

                    Console.Write("Enter Call Type (optional, press Enter to skip): ");
                    callTypeInput = Console.ReadLine();
                    callType = null;

                    if (!string.IsNullOrWhiteSpace(callTypeInput) && Enum.TryParse(callTypeInput, out parsedCallType))
                    {
                        callType = parsedCallType;
                    }

                    Console.Write("Enter Sorting Field (optional, press Enter to skip): ");
                    sortByInput = Console.ReadLine();
                    BO.ClosedCallInListFields? sortByClosedField = null;

                    if (!string.IsNullOrWhiteSpace(sortByInput) && Enum.TryParse(sortByInput, out BO.ClosedCallInListFields parsedClosedSortFieldC))
                    {
                        sortByClosedField = parsedClosedSortFieldC;
                    }

                    var closedCalls = s_bl.Call.GetClosedCallsByVolunteer(volunteerId, callType, sortByClosedField);

                    Console.WriteLine("\nClosed Calls:");
                    foreach (var call in closedCalls)
                    {
                        Console.WriteLine(call);
                    }

                    break;

                case "12":
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


        static void CreateEninty(string type)
        {
            switch (type)
            {
                case "volunteer":
                    // יצירת מתנדב חדש
                    Console.WriteLine("Enter ID:");
                    if (!int.TryParse(Console.ReadLine(), out int Idvolunteer))
                    {
                        Console.WriteLine("Invalid ID input.");
                        return;
                    }

                    string prompt(string fieldName)
                    {
                        Console.Write($"{fieldName}: ");
                        return Console.ReadLine();
                    }

                    string fullName = prompt("Enter Full Name");
                    string phone = prompt("Enter Phone Number");
                    string email = prompt("Enter Email");
                    string address = prompt("Enter Address");
                    string isAvailableInput = prompt("Is Available (true/false, default is false): ");
                    bool isAvailable = string.IsNullOrWhiteSpace(isAvailableInput) ? false : bool.Parse(isAvailableInput);

                    string roleInput = prompt("Enter Role (default is Volunteer)");
                    BO.Role role = string.IsNullOrWhiteSpace(roleInput) ? BO.Role.Volunteer : Enum.Parse<BO.Role>(roleInput);

                    string distanceTypeInput = prompt("Enter Distance Type (default is Air)");
                    BO.DistanceType distanceType = string.IsNullOrWhiteSpace(distanceTypeInput) ? BO.DistanceType.Air : Enum.Parse<BO.DistanceType>(distanceTypeInput);

                    string maxDistanceInput = prompt("Enter Max Distance");
                    double? maxDistance = string.IsNullOrWhiteSpace(maxDistanceInput) ? null : double.Parse(maxDistanceInput);

                    string password = prompt("Enter Password");

                    BO.Volunteer volunteerToSave = new BO.Volunteer
                    {
                        Id = Idvolunteer,
                        FullName = fullName,
                        Phone = phone,
                        Email = email,
                        Address = address,
                        IsActive = isAvailable,
                        Role = role,
                        DistanceType = distanceType,
                        MaxCallDistance = maxDistance,
                        Latitude = 0.0,
                        Longitude = 0.0,
                        Password = password
                    };

                    s_bl.Volunteer!.Create(volunteerToSave);
                    Console.WriteLine("Volunteer successfully created!");
                    break;

                case "call":
                    // יצירת שיחה חדשה
                    Console.Write("Enter Call Id: ");
                    int idCall = int.Parse(Console.ReadLine());

                    Console.Write("Enter Call Type (1 for Emergency, 2 for Regular): ");
                    BO.CallType callType = (BO.CallType)Enum.Parse(typeof(BO.CallType), Console.ReadLine());

                    Console.Write("Enter Address: ");
                    string addressCall = Console.ReadLine();

                    Console.Write("Enter Open Date (yyyy-MM-dd): ");
                    DateTime openDate = DateTime.Parse(Console.ReadLine());

                    Console.Write("Enter Description : ");
                    string description = Console.ReadLine();

                    Console.Write("Enter Max Time to Finish (yyyy-MM-dd): ");
                    DateTime? maxTimeFinish = string.IsNullOrWhiteSpace(Console.ReadLine()) ? null : DateTime.Parse(Console.ReadLine());

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
                    Console.WriteLine("Call successfully created!");
                    break;
            }
        }

        private static void UpdateEninty(string entityName)
        {
            try
            {
                int id;
                if (entityName == "volunteer")
                {
                    Console.WriteLine("Enter the ID of the volunteer to update:");
                    id = int.Parse(Console.ReadLine());
                    BO.Volunteer volunteer = s_bl!.Volunteer.Read(id);
                    if (volunteer == null)
                    {
                        Console.WriteLine("Volunteer not found.");
                        return;
                    }

                    // Requesting new values from the user and updating the volunteer object
                    Console.WriteLine("Enter new full name (leave empty to keep current):");
                    string newFullName = Console.ReadLine();
                    volunteer.FullName = string.IsNullOrEmpty(newFullName) ? volunteer.FullName : newFullName;

                    Console.WriteLine("Enter new phone number (leave empty to keep current):");
                    string newPhone = Console.ReadLine();
                    volunteer.Phone = string.IsNullOrEmpty(newPhone) ? volunteer.Phone : newPhone;

                    Console.WriteLine("Enter new email (leave empty to keep current):");
                    string newEmail = Console.ReadLine();
                    volunteer.Email = string.IsNullOrEmpty(newEmail) ? volunteer.Email : newEmail;

                    Console.WriteLine("Enter new password (leave empty to keep current):");
                    string newPassword = Console.ReadLine();
                    volunteer.Password = string.IsNullOrEmpty(newPassword) ? volunteer.Password: newPassword;

                    Console.WriteLine("Enter new current address (leave empty to keep current):");
                    string newCurrentAddress = Console.ReadLine();
                    volunteer.Address = string.IsNullOrEmpty(newCurrentAddress) ? volunteer.Address : newCurrentAddress;

                    Console.WriteLine("Enter new latitude (leave empty to keep current):");
                    string newLatitude = Console.ReadLine();
                    if (double.TryParse(newLatitude, out double latitude))
                    {
                        volunteer.Latitude = latitude;
                    }

                    Console.WriteLine("Enter new longitude (leave empty to keep current):");
                    string newLongitude = Console.ReadLine();
                    if (double.TryParse(newLongitude, out double longitude))
                    {
                        volunteer.Longitude = longitude;
                    }

                    Console.WriteLine("Enter new maximum distance (leave empty to keep current):");
                    string newMaxDistance = Console.ReadLine();
                    if (double.TryParse(newMaxDistance, out double maxDistance))
                    {
                        volunteer.MaxCallDistance = maxDistance;
                    }

                    Console.WriteLine("Enter new distance type (leave empty to keep current):");
                    string newDistanceType = Console.ReadLine();
                    if (!string.IsNullOrEmpty(newDistanceType))
                    {
                        volunteer.DistanceType = (BO.DistanceType)Enum.Parse(typeof(BO.DistanceType), newDistanceType);
                    }

                    Console.WriteLine("Enter new role (leave empty to keep current):");
                    string newRole = Console.ReadLine();
                    if (!string.IsNullOrEmpty(newRole))
                    {
                        volunteer.Role = (BO.Role)Enum.Parse(typeof(BO.Role), newRole);
                    }

                    Console.WriteLine("Enter new active status (leave empty to keep current):");
                    string newIsActive = Console.ReadLine();
                    if (!string.IsNullOrEmpty(newIsActive) && bool.TryParse(newIsActive, out bool isActive))
                    {
                        volunteer.IsActive = isActive;
                    }

                    // Save updated volunteer
                    s_bl!.Volunteer.Update(id, volunteer);
                    Console.WriteLine("Volunteer updated successfully.");
                }

                else if (entityName == "call")
                {
                    Console.WriteLine("Enter the ID of the call to update:");
                    id = int.Parse(Console.ReadLine());
                    var call = s_bl!.Call.Read(id);
                    if (call == null)
                    {
                        Console.WriteLine("Call not found.");
                        return;
                    }

                    // Requesting new values for the call
                    Console.WriteLine("Enter new full address (leave empty to keep current):");
                    string newFullAddress = Console.ReadLine();
                    call.FullAddress = string.IsNullOrEmpty(newFullAddress) ? call.FullAddress : newFullAddress;

                    Console.WriteLine("Enter new description (leave empty to keep current):");
                    string newDescription = Console.ReadLine();
                    call.Description = string.IsNullOrEmpty(newDescription) ? call.Description : newDescription;

                    Console.WriteLine("Enter new latitude (leave empty to keep current):");
                    string newLatitude = Console.ReadLine();
                    if (double.TryParse(newLatitude, out double latitude))
                    {
                        call.Latitude = latitude;
                    }

                    Console.WriteLine("Enter new longitude (leave empty to keep current):");
                    string newLongitude = Console.ReadLine();
                    if (double.TryParse(newLongitude, out double longitude))
                    {
                        call.Longitude = longitude;
                    }

                    Console.WriteLine("Enter new emergency status (leave empty to keep current):");
                    string newCallStatus = Console.ReadLine();
                    if (!string.IsNullOrEmpty(newCallStatus))
                    {
                        call.Status = (BO.CallStatus)Enum.Parse(typeof(BO.CallStatus), newCallStatus);
                    }

                    Console.WriteLine("Enter new Call Type (leave empty to keep current):");
                    string newCallType = Console.ReadLine();
                    if (!string.IsNullOrEmpty(newCallType))
                    {
                        call.CallType = (BO.CallType)Enum.Parse(typeof(BO.CallType), newCallType);
                    }

                   

                    Console.WriteLine("Enter new max completion time (leave empty to keep current):");
                    string newMaxCompletionTime = Console.ReadLine();
                    if (DateTime.TryParse(newMaxCompletionTime, out DateTime maxCompletionTime))
                    {
                        call.MaxCompletionTime = maxCompletionTime;
                    }

                    // Save updated call
                    s_bl!.Call.UpdateCallDetails(call);
                    Console.WriteLine("Call updated successfully.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
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
                    Console.WriteLine($"volunteer with id{Id} deleted successfully");
                    break;
                case "call":
                    s_bl.Call!.DeleteCall(Id);
                    Console.WriteLine($"call with id {Id} deleted successfully");
                    break;
                default:
                    Console.WriteLine("NO SUCH ENTITY");
                    break;
            }
        }

    }
}