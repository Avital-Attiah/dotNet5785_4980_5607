using Dal;
using DalApi;
using DO;
using System.ComponentModel.Design;
using System.Numerics;
using static DO.Enums;
namespace DalTest;


internal class Program
{

    //static readonly IDal s_dal = new DalList(); //stage 2
    //static readonly IDal s_dal = new DalXml(); //stage 3
    static readonly IDal s_dal = Factory.Get; //stage 4

    // עבור תפריט ראשי Enum 
    private enum MainMenuOption
    {
        Exit,
        ManageVolunteers,
        ManageCalls,
        ManageAssignments,
        ManageConfig,
        InitializeDatabase,
        ShowAllData,
        ConfigurationMenu,
        ResetDatabase
    }

    //CRUD לתתי-תפריטים עבור ENUM 
    private enum CrudMenuOption
    {
        Exit,
        Create,
        Read,
        ReadAll,
        Update,
        Delete,
        DeleteAll
    }

    // Enum עבור תפריט תצורה
    enum ConfigMenuOptions
    {
        Exit,
        AdvanceClockByMinute,
        AdvanceClockByHour,
        AdvanceClockByDay,
        AdvanceClockByYear,
        ShowCurrentClock,
        SetConfigValue,
        ShowConfigValue,
        ResetConfig
    }
    //תפריט הקונפגרציה
    private static void ShowConfigMenu()
    {
        bool exit = false;
        while (!exit)
        {
            Console.WriteLine("\n--- Configuration Submenu ---");
            Console.WriteLine("0. Exit");
            Console.WriteLine("1. Advance System Clock by One Minute");
            Console.WriteLine("2. Advance System Clock by One Hour");
            Console.WriteLine("3. Advance System Clock by One Day");
            Console.WriteLine("4. Advance System Clock by One Year");
            Console.WriteLine("5. Show Current Time");
            Console.WriteLine("6. Set Configuration Value");
            Console.WriteLine("7. Display Configuration Value");
            Console.WriteLine("8. Reset Configuration");
            Console.WriteLine("Choose an option from Configuration Submenu:");

            // קורא את קלט המשתמש
            if (Enum.TryParse<ConfigMenuOptions>(Console.ReadLine(), out var selectedOption))
            {
                switch (selectedOption)
                {
                    case ConfigMenuOptions.Exit:
                        Console.WriteLine("Exiting the configuration menu.");
                        exit = true;
                        break;

                    case ConfigMenuOptions.AdvanceClockByMinute:
                        // מקדם את השעון בדקה
                        AdvanceClockByMinute();
                        break;

                    case ConfigMenuOptions.AdvanceClockByHour:
                        // מקדם את השעון בשעה
                        AdvanceClockByHour();
                        break;

                    case ConfigMenuOptions.AdvanceClockByDay:
                        // מקדם את השעון ביום
                        AdvanceClockByDay();
                        break;
                    case ConfigMenuOptions.AdvanceClockByYear:
                        // מקדם את השעון בשנה
                        AdvanceClockByYear();
                        break;
                    case ConfigMenuOptions.ShowCurrentClock:
                        // מראה שעה נוכחית
                        ShowCurrentTime();
                        break;

                    case ConfigMenuOptions.SetConfigValue:
                        // מגדיר ערך בהגדרות
                        SetConfigValue();
                        break;

                    case ConfigMenuOptions.ShowConfigValue:
                        //  מציג ערך בהגדרות
                        ShowConfigValue();
                        break;

                    case ConfigMenuOptions.ResetConfig:
                        // מאפס הגדרות
                        ResetConfig();
                        break;

                    default:
                        Console.WriteLine("Invalid option. Please select a valid option.");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a number between 0 and 6.");
            }
        }
    }

    private static void AdvanceClockByMinute()
    {
        // מוסיף דקה אחת לשעון המערכת
        DateTime currentTime = s_dal.Config!.Clock;
        DateTime newTime = currentTime.AddMinutes(1);
        Console.WriteLine($"System time advanced by 1 minute. New time: {newTime}");
    }

    private static void AdvanceClockByHour()
    {
        // מוסיף שעה אחת לשעון המערכת
        DateTime currentTime = s_dal.Config!.Clock;
        DateTime newTime = currentTime.AddHours(1);
        Console.WriteLine($"System time advanced by 1 hour. New time: {newTime}");
    }
    private static void AdvanceClockByDay()
    {
        // מוסיף יום אחד לשעון המערכת
        DateTime currentTime = s_dal.Config!.Clock;
        DateTime newTime = currentTime.AddDays(1);
        Console.WriteLine($"System time advanced by 1 Day. New time: {newTime}");
    }
    private static void AdvanceClockByYear()
    {
        // מוסיף שנה אחת לשעון המערכת
        DateTime currentTime = s_dal.Config!.Clock;
        DateTime newTime = currentTime.AddYears(1);
        Console.WriteLine($"System time advanced by 1 Year. New time: {newTime}");
    }

    private static void ShowCurrentTime()
    {
        // מראה שעה נוכחית
        DateTime currentTime = s_dal.Config!.Clock;
        Console.WriteLine($"Current system time: {currentTime}");


    }

    private static void SetConfigValue()
    {
        // מגדיר ערך בהגדרות
        Console.WriteLine("Enter the name of the configuration parameter to set:");
        string configName = Console.ReadLine();
        Console.WriteLine($"Enter the value for {configName}:");
        string configValue = Console.ReadLine();
        // Store the value (could be saved to a file or database in a real scenario)
        Console.WriteLine($"Configuration {configName} set to {configValue}");
    }

    private static void ShowConfigValue()
    {
        // מציג ערך בהגדרות
        Console.WriteLine("Enter the name of the configuration parameter to display:");
        string configName = Console.ReadLine();
        // Retrieve the value (could be read from a file or database in a real scenario)
        Console.WriteLine($"The value of {configName} is: [Dummy Value]");
    }

    private static void ResetConfig()
    {
        s_dal.Config!.Reset();
        // מאפס את כל ההגדרות לערכים ברירת המחדל שלהן 
        Console.WriteLine("All configuration values have been reset to their defaults.");

    }


    private static void InitializeData()
    {
        try
        {
            Console.WriteLine("Initializing data...");
            // קריאה ל-Initialization.Do עם האובייקטים המתאימים
            //Initialization.Do(s_dal); //stage 2
            Initialization.Do(); //stage 4
            Console.WriteLine("Initialization completed successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

    }

    private static void ShowAllData()
    {
        Console.WriteLine("Displaying all data:");
        Console.WriteLine("Volunteers:");
        foreach (var volunteer in s_dal!.Volunteer.ReadAll())
        {
            s_dal!.Volunteer.Print(volunteer);
        }
        Console.WriteLine("Calls:");
        foreach (var call in s_dal!.Call.ReadAll())
        {
            s_dal!.Call.Print(call);
        }
        Console.WriteLine("Assignments:");
        foreach (var assignment in s_dal!.Assignment.ReadAll())
        {
            s_dal!.Assignment.Print(assignment);
        }
    }

    private static void ResetDatabase()
    {
        Console.WriteLine("Resetting database...");
        s_dal!.Volunteer.DeleteAll();
        s_dal!.Call.DeleteAll();
        s_dal!.Assignment.DeleteAll();
        s_dal!.Config.Reset();
        Console.WriteLine("Database reset completed.");
    }



    private static void ShowEntityMenu(string entityName)
    {
        bool validChoice = false;

        while (!validChoice)
        {
            try
            {
                // הצגת תפריט CRUD עבור הישות
                Console.WriteLine("Menu of " + entityName + ":");
                Console.WriteLine("0. Exit");
                Console.WriteLine("1. Create");
                Console.WriteLine("2. Read");
                Console.WriteLine("3. ReadAll");
                Console.WriteLine("4. Update");
                Console.WriteLine("5. Delete");
                Console.WriteLine("6. DeleteAll");
                Console.Write("Select option: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "0":
                        validChoice = true;  // יוצאים מהלולאה (ויוצאים לתפריט הראשי)
                        break;
                    case "1":
                        CreateEntity(entityName);
                        break;
                    case "2":
                        ReadEntity(entityName);
                        break;
                    case "3":
                        ReadAllEntities(entityName);
                        break;
                    case "4":
                        UpdateEntity(entityName);
                        break;
                    case "5":
                        DeleteEntity(entityName);
                        break;
                    case "6":
                        DeleteAllEntities(entityName);
                        break;
                    default:
                        Console.WriteLine("Invalid option, please try again.");
                        break;
                }

                // כאן אתה נשאר בתפריט אם לא בחרת "Exit"
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An exception occurred: {ex.Message}");
            }
        }
    }




    private static void CreateEntity(string entityName)
    {
        bool isCreated = false;
        while (!isCreated)
        {
            if (entityName == "Volunteers")
            {
                Console.WriteLine("enter id");
                int Id = int.Parse(Console.ReadLine());
                Console.WriteLine("enter full name");
                string FullName = Console.ReadLine();
                Console.WriteLine("enter phone");
                string Phone = Console.ReadLine();
                Console.WriteLine("enter Email");
                string Email = Console.ReadLine();
                Console.WriteLine("enter password");
                string? Password = Console.ReadLine();
                Console.WriteLine("enter Current Address");
                string? CurrentAddress = Console.ReadLine();
                Console.WriteLine("enter Latitude");
                double? Latitude = double.Parse(Console.ReadLine());
                Console.WriteLine("enter Longitude");
                double? Longitude = double.Parse(Console.ReadLine());
                Console.WriteLine("enter MaxDistance");
                double? MaxDistance = double.Parse(Console.ReadLine());
                Console.WriteLine("enter Y if is active and N uf not");
                bool IsActive = false;
                string Active = Console.ReadLine();
                if (Active == "Y")
                    IsActive = true;
                Console.WriteLine("enter Air if the Distance type is air,walking for Walking and Car for car");
                DistanceType DistanceT = DistanceType.Air; // או ערך חוקי אחר מתוך ה־enum
                string Distance = Console.ReadLine();
                if (Distance == "Air")
                    DistanceT = DistanceType.Air;
                if (Distance == "Walking")
                    DistanceT = DistanceType.Walking;
                if (Distance == "Car")
                    DistanceT = DistanceType.Car;
                Role Rol = Role.Volunteer;
                Console.WriteLine("enter V for volunteer and M for manager");
                string role = Console.ReadLine();
                if (role == "V")
                    Rol = Role.Volunteer;
                if (role == "M")
                    Rol = Role.Manager;

                Volunteer newVolunteer = new Volunteer
                {
                    Id = Id,
                    FullName = FullName,
                    Phone = Phone,
                    Email = Email,
                    Password = Password,
                    CurrentAddress = CurrentAddress,
                    Latitude = Latitude,
                    Longitude = Longitude,
                    MaxDistance = MaxDistance,
                    IsActive = IsActive,
                    DistanceType = DistanceT,
                    Role = Rol
                };
                try
                {
                    s_dal!.Volunteer.Create(newVolunteer);
                    Console.WriteLine("Volunteer added successfully!");
                    isCreated = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }


            }

            if (entityName == "Calls")
            {
                Console.WriteLine("enter full adress");
                string FullAddress = Console.ReadLine();
                Console.WriteLine("enter Latitude");
                double? Latitude = double.Parse(Console.ReadLine());
                Console.WriteLine("enter Longitude");
                double? Longitude = double.Parse(Console.ReadLine());
                Console.WriteLine("enter Description");
                string Description = Console.ReadLine();
                Console.WriteLine("enter Y if is Emergency and N if not");
                bool isEmergency = false;
                string Active = Console.ReadLine();
                if (Active == "Y")
                    isEmergency = true;
                Console.WriteLine("Enter the call open time (format: yyyy-MM-dd HH:mm):");
                DateTime openTime = DateTime.Parse(Console.ReadLine());
                Console.WriteLine("Enter the max completion time (format: yyyy-MM-dd HH:mm, or press Enter to skip):");
                string completionTimeInput = Console.ReadLine();
                DateTime? maxCompletionTime = string.IsNullOrEmpty(completionTimeInput) ? null : DateTime.Parse(completionTimeInput);
                Console.WriteLine("enter Emotional for Emotional-Support,Family for Family-Support and Professional for ProfessionalConsultation");
                string calltipe = Console.ReadLine();
                CallType CallT = CallType.EmotionalSupport;
                if (calltipe == "Emotional")
                    CallT = CallType.EmotionalSupport;
                if (calltipe == "Family")
                    CallT = CallType.FamilySupport;
                if (calltipe == "Professional")
                    CallT = CallType.ProfessionalConsultation;



                Call newCall = new Call
                {
                    FullAddress = FullAddress,
                    CallType = CallT,
                    Description = Description,
                    isEmergency = isEmergency,
                    Latitude = (double)Latitude,
                    Longitude = (double)Longitude,
                    OpenTime = openTime,
                    MaxCompletionTime = maxCompletionTime
                };
                try
                {
                    s_dal!.Call.Create(newCall);
                    Console.WriteLine("Call added successfully!");
                    isCreated = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }


            }



            if (entityName == "Assignments")
            {
                Console.WriteLine("enter Call Id");
                int CallId = int.Parse(Console.ReadLine());
                Console.WriteLine("enter Volunteer Id");
                int VolunteerId = int.Parse(Console.ReadLine());
                Console.WriteLine("enter Complete if the status is Complete,CanceledByV for Canceled By Volunteer,CanceledBym for Canceled By manager and Expired for Expired");
                TreatmentStatus Status = TreatmentStatus.CompletedOnTime; // או ערך חוקי אחר מתוך ה־enum
                string st = Console.ReadLine();
                if (st == "Complete")
                    Status = TreatmentStatus.CompletedOnTime;
                if (st == "CanceledByV")
                    Status = TreatmentStatus.CanceledByVolunteer;
                if (st == "CanceledBym")
                    Status = TreatmentStatus.CanceledByManager;
                if (st == "Expired")
                    Status = TreatmentStatus.Expired;
                Console.WriteLine("Enter the assignment entry time (format: yyyy-MM-dd HH:mm):");
                DateTime entryTime = DateTime.Parse(Console.ReadLine());
                Console.WriteLine("Enter the completion time (format: yyyy-MM-dd HH:mm, or press Enter to skip):");
                string completionTimeInput = Console.ReadLine();
                DateTime? completionTime = string.IsNullOrEmpty(completionTimeInput) ? null : DateTime.Parse(completionTimeInput);

                Assignment newAssignment = new Assignment
                {
                    
                    CallId = CallId,
                    VolunteerId = VolunteerId,
                    Status = Status,
                    EntryTime = entryTime,
                    CompletionTime = completionTime

                };
                try
                {
                    s_dal!.Assignment.Create(newAssignment);
                    Console.WriteLine("Assignment added successfully!");
                    isCreated = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }

            }

        }


    }


    private static void DeleteAllEntities(string entityName)
    {
        try
        {
            s_dal!.Assignment.DeleteAll();
            s_dal!.Call.DeleteAll();
            s_dal!.Volunteer.DeleteAll();
            Console.WriteLine("All Entities were deleted!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

    }

    private static void DeleteEntity(string entityName)
    {
        try
        {
            int id;
            if (entityName == "Volunteers")
            {
                Console.WriteLine("enter the id of the volunteer");
                id = int.Parse(Console.ReadLine());
                s_dal!.Volunteer.Delete(id);
                Console.WriteLine("The volunteer was deleted");
            }

            if (entityName == "Calls")
            {
                Console.WriteLine("enter the id of the call");
                id = int.Parse(Console.ReadLine());
                s_dal!.Call.Delete(id);
                Console.WriteLine("The call was deleted");
            }
            if (entityName == "Assignments")
            {
                Console.WriteLine("enter the id of the Assignment");
                id = int.Parse(Console.ReadLine());
                s_dal!.Assignment.Delete(id);
                Console.WriteLine("The Assignment was deleted");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }


    }

    private static void UpdateEntity(string entityName)
    {
        try
        {
            int id;
            if (entityName == "Volunteers")
            {
                Console.WriteLine("Enter the ID of the volunteer to update:");
                id = int.Parse(Console.ReadLine());
                Volunteer volunteer = s_dal!.Volunteer.Read(id);
                if (volunteer == null)
                {
                    Console.WriteLine("Volunteer not found.");
                    return;
                }
                Console.WriteLine("Current volunteer details:");
                s_dal!.Volunteer.Print(volunteer);

                // Requesting new values from the user
                Console.WriteLine("Enter new full name (leave empty to keep current):");
                string newFullName = Console.ReadLine();
                var updatedVolunteer = string.IsNullOrEmpty(newFullName) ? volunteer : volunteer with { FullName = newFullName };

                Console.WriteLine("Enter new phone number (leave empty to keep current):");
                string newPhone = Console.ReadLine();
                updatedVolunteer = string.IsNullOrEmpty(newPhone) ? updatedVolunteer : updatedVolunteer with { Phone = newPhone };

                Console.WriteLine("Enter new email (leave empty to keep current):");
                string newEmail = Console.ReadLine();
                updatedVolunteer = string.IsNullOrEmpty(newEmail) ? updatedVolunteer : updatedVolunteer with { Email = newEmail };

                Console.WriteLine("Enter new password (leave empty to keep current):");
                string newPassword = Console.ReadLine();
                updatedVolunteer = string.IsNullOrEmpty(newPassword) ? updatedVolunteer : updatedVolunteer with { Password = newPassword };

                Console.WriteLine("Enter new current address (leave empty to keep current):");
                string newCurrentAddress = Console.ReadLine();
                updatedVolunteer = string.IsNullOrEmpty(newCurrentAddress) ? updatedVolunteer : updatedVolunteer with { CurrentAddress = newCurrentAddress };

                Console.WriteLine("Enter new latitude (leave empty to keep current):");
                string newLatitude = Console.ReadLine();
                if (double.TryParse(newLatitude, out double latitude))
                {
                    updatedVolunteer = updatedVolunteer with { Latitude = latitude };
                }

                Console.WriteLine("Enter new longitude (leave empty to keep current):");
                string newLongitude = Console.ReadLine();
                if (double.TryParse(newLongitude, out double longitude))
                {
                    updatedVolunteer = updatedVolunteer with { Longitude = longitude };
                }

                Console.WriteLine("Enter new maximum distance (leave empty to keep current):");
                string newMaxDistance = Console.ReadLine();
                if (double.TryParse(newMaxDistance, out double maxDistance))
                {
                    updatedVolunteer = updatedVolunteer with { MaxDistance = maxDistance };
                }

                Console.WriteLine("Enter new distance type (leave empty to keep current):");
                string newDistanceType = Console.ReadLine();
                if (!string.IsNullOrEmpty(newDistanceType))
                {
                    var distanceType = (DistanceType)Enum.Parse(typeof(DistanceType), newDistanceType);
                    updatedVolunteer = updatedVolunteer with { DistanceType = distanceType };
                }

                Console.WriteLine("Enter new role (leave empty to keep current):");
                string newRole = Console.ReadLine();
                if (!string.IsNullOrEmpty(newRole))
                {
                    var role = (Role)Enum.Parse(typeof(Role), newRole);
                    updatedVolunteer = updatedVolunteer with { Role = role };
                }

                Console.WriteLine("Enter new active status (leave empty to keep current):");
                string newIsActive = Console.ReadLine();
                if (!string.IsNullOrEmpty(newIsActive) && bool.TryParse(newIsActive, out bool isActive))
                {
                    updatedVolunteer = updatedVolunteer with { IsActive = isActive };
                }

                // שומר מתנדב מעודכן
                s_dal!.Volunteer.Update(updatedVolunteer);
                Console.WriteLine("Volunteer updated successfully.");
            }

            else if (entityName == "Calls")
            {
                Console.WriteLine("Enter the ID of the call to update:");
                id = int.Parse(Console.ReadLine());
                var call = s_dal!.Call.Read(id);
                if (call == null)
                {
                    Console.WriteLine("Call not found.");
                    return;
                }
                Console.WriteLine("Current call details:");
                s_dal!.Call.Print(call);

                // מבקש נתונים חדשים לקריאה
                Console.WriteLine("Enter new full address (leave empty to keep current):");
                string newFullAddress = Console.ReadLine();
                var updatedCall = string.IsNullOrEmpty(newFullAddress) ? call : call with { FullAddress = newFullAddress };

                Console.WriteLine("Enter new description (leave empty to keep current):");
                string newDescription = Console.ReadLine();
                updatedCall = string.IsNullOrEmpty(newDescription) ? updatedCall : updatedCall with { Description = newDescription };

                Console.WriteLine("Enter new latitude (leave empty to keep current):");
                string newLatitude = Console.ReadLine();
                if (double.TryParse(newLatitude, out double latitude))
                {
                    updatedCall = updatedCall with { Latitude = latitude };
                }

                Console.WriteLine("Enter new longitude (leave empty to keep current):");
                string newLongitude = Console.ReadLine();
                if (double.TryParse(newLongitude, out double longitude))
                {
                    updatedCall = updatedCall with { Longitude = longitude };
                }

                Console.WriteLine("Enter new emergency status (leave empty to keep current):");
                string newEmergency = Console.ReadLine();
                if (!string.IsNullOrEmpty(newEmergency) && bool.TryParse(newEmergency, out bool isEmergency))
                {
                    updatedCall = updatedCall with { isEmergency = isEmergency };
                }


                Console.WriteLine("Enter new max completion time (leave empty to keep current):");
                string newMaxCompletionTime = Console.ReadLine();
                if (DateTime.TryParse(newMaxCompletionTime, out DateTime maxCompletionTime))
                {
                    updatedCall = updatedCall with { MaxCompletionTime = maxCompletionTime };
                }

                // מעעדכן את נתוני הקריאה
                s_dal!.Call.Update(updatedCall);
                Console.WriteLine("Call updated successfully.");
            }

            else if (entityName == "Assignments")
            {
                Console.WriteLine("Enter the ID of the assignment to update:");
                id = int.Parse(Console.ReadLine());
                var assignment = s_dal!.Assignment.Read(id);
                if (assignment == null)
                {
                    Console.WriteLine("Assignment not found.");
                    return;
                }
                Console.WriteLine("Current assignment details:");
                s_dal!.Assignment.Print(assignment);


                Console.WriteLine("Enter new completion time (leave empty to keep current):");
                string newCompletionTime = Console.ReadLine();
                var updatedAssignment = DateTime.TryParse(newCompletionTime, out DateTime completionTime) ?
                                        assignment with { CompletionTime = completionTime } :
                                        assignment;

                Console.WriteLine("Enter new status (leave empty to keep current):");
                string newStatus = Console.ReadLine();
                if (!string.IsNullOrEmpty(newStatus))
                {
                    var status = (TreatmentStatus)Enum.Parse(typeof(TreatmentStatus), newStatus);
                    updatedAssignment = updatedAssignment with { Status = status };
                }

                // שומר עידכונים עבור הקצאה
                s_dal!.Assignment.Update(updatedAssignment);
                Console.WriteLine("Assignment updated successfully.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }




    private static void ReadAllEntities(string entityName)
    {
        try
        {
            if (entityName == "Volunteers")
            {
                Console.WriteLine("The Volunteers data:");
                foreach (var volunteer in s_dal!.Volunteer.ReadAll())
                {
                    s_dal!.Volunteer.Print(volunteer);
                }
            }
            else if (entityName == "Assignments")
            {

                Console.WriteLine("The Assignments data:");
                foreach (var assignment in s_dal!.Assignment.ReadAll())
                {
                    s_dal!.Assignment.Print(assignment);
                }
            }
            else if (entityName == "Calls")
            {
                Console.WriteLine("The Calls data:");
                foreach (var call in s_dal!.Call.ReadAll())
                {
                    s_dal!.Call.Print(call);
                }
            }


        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }



    private static void ReadEntity(string entityName)
    {
        try
        {
            int id;
            if (entityName == "Volunteers")
            {
                Console.WriteLine("enter the id of the volunteer");
                id = int.Parse(Console.ReadLine());
                Volunteer v = s_dal!.Volunteer.Read(id);
                s_dal!.Volunteer.Print(v);
            }

            if (entityName == "Calls")
            {
                Console.WriteLine("enter the id of the call");
                id = int.Parse(Console.ReadLine());
                Call c = s_dal!.Call.Read(id);
                s_dal!.Call.Print(c);
            }
            if (entityName == "Assignment")
            {
                Console.WriteLine("enter the id of the Assignment");
                id = int.Parse(Console.ReadLine());
                Assignment a = s_dal!.Assignment.Read(id);
                s_dal!.Assignment.Print(a);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }


    private static void ShowMainMenu()
    {
        Console.WriteLine("\n--- Main Menu ---");
        Console.WriteLine("0. Exit");
        Console.WriteLine("1. Submenu for Volunteer");
        Console.WriteLine("2. Submenu for Calls");
        Console.WriteLine("3. Submenu for Assignments");
        Console.WriteLine("4. Initialize Data");
        Console.WriteLine("5. Display All Data in Database");
        Console.WriteLine("6. Configuration Menu");
        Console.WriteLine("7. Reset Database and Configuration Data");
    }



    static void Main(string[] args)
    {
        bool exit = false;

        while (!exit)
        {
            try
            {

                ShowMainMenu();
                Console.Write("Choose an option from the main menu: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "0":
                        exit = true;
                        break;
                    case "1":
                        ShowEntityMenu("Volunteers");
                        break;
                    case "2":
                        ShowEntityMenu("Calls");
                        break;
                    case "3":
                        ShowEntityMenu("Assignments");
                        break;
                    case "4":
                        InitializeData();
                        break;
                    case "5":
                        ShowAllData();
                        break;
                    case "6":
                        ShowConfigMenu();
                        break;
                    case "7":
                        ResetDatabase();
                        break;
                    default:
                        Console.WriteLine("Invalid option, please try again.");
                        break;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"An exception occurred: {ex.Message}");
            }
        }
    }
}