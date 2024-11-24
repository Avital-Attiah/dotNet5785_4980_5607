using Dal;
using DalApi;
using DO;
using System.Numerics;
using static DO.Enums;
namespace DalTest;

internal class Program
{
    private static IVolunteer? s_dalVolunteer = new VolunteerImplementation(); //stage 1
    private static ICall? s_dalCall = new CallImplementation(); //stage 1
    private static IAssignment? s_dalAssignment = new AssignmentImplementation(); //stage 1
    private static IConfig? s_dalConfig = new ConfigImplementation();

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
        ShowCurrentClock,
        SetConfigValue,
        ShowConfigValue,
        ResetConfig
    }

    private static void ShowConfigMenu()
    {
        bool exit = false;
        while (!exit)
        {
            Console.WriteLine("\n--- Configuration Submenu ---");
            Console.WriteLine("0. Exit");
            Console.WriteLine("1. Advance System Clock by One Minute");
            Console.WriteLine("2. Advance System Clock by One Hour");
            Console.WriteLine("3. Show Current Time");
            Console.WriteLine("4. Set Configuration Value");
            Console.WriteLine("5. Display Configuration Value");
            Console.WriteLine("6. Reset Configuration");
            Console.WriteLine("Choose an option from Configuration Submenu:");

            // Read user input
            if (Enum.TryParse<ConfigMenuOptions>(Console.ReadLine(), out var selectedOption))
            {
                switch (selectedOption)
                {
                    case ConfigMenuOptions.Exit:
                        Console.WriteLine("Exiting the configuration menu.");
                        exit = true;
                        break;

                    case ConfigMenuOptions.AdvanceClockByMinute:
                        // Advance the clock by one minute
                        AdvanceClockByMinute();
                        break;

                    case ConfigMenuOptions.AdvanceClockByHour:
                        // Advance the clock by one hour
                        AdvanceClockByHour();
                        break;

                    case ConfigMenuOptions.ShowCurrentClock:
                        // Show the current system time
                        ShowCurrentTime();
                        break;

                    case ConfigMenuOptions.SetConfigValue:
                        // Set a configuration value
                        SetConfigValue();
                        break;

                    case ConfigMenuOptions.ShowConfigValue:
                        // Display a configuration value
                        ShowConfigValue();
                        break;

                    case ConfigMenuOptions.ResetConfig:
                        // Reset configuration
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
        // Add one minute to the system clock (dummy logic for demonstration)
        DateTime currentTime = DateTime.Now;
        DateTime newTime = currentTime.AddMinutes(1);
        Console.WriteLine($"System time advanced by 1 minute. New time: {newTime}");
    }

    private static void AdvanceClockByHour()
    {
        // Add one hour to the system clock (dummy logic for demonstration)
        DateTime currentTime = DateTime.Now;
        DateTime newTime = currentTime.AddHours(1);
        Console.WriteLine($"System time advanced by 1 hour. New time: {newTime}");
    }

    private static void ShowCurrentTime()
    {
        // Display current system time
        DateTime currentTime = DateTime.Now;
        Console.WriteLine($"Current system time: {currentTime}");
    }

    private static void SetConfigValue()
    {
        // Set a configuration value (dummy logic for demonstration)
        Console.WriteLine("Enter the name of the configuration parameter to set:");
        string configName = Console.ReadLine();
        Console.WriteLine($"Enter the value for {configName}:");
        string configValue = Console.ReadLine();
        // Store the value (could be saved to a file or database in a real scenario)
        Console.WriteLine($"Configuration {configName} set to {configValue}");
    }

    private static void ShowConfigValue()
    {
        // Display a configuration value (dummy logic for demonstration)
        Console.WriteLine("Enter the name of the configuration parameter to display:");
        string configName = Console.ReadLine();
        // Retrieve the value (could be read from a file or database in a real scenario)
        Console.WriteLine($"The value of {configName} is: [Dummy Value]");
    }

    private static void ResetConfig()
    {
        // Reset all configurations to their default values (dummy logic for demonstration)
        Console.WriteLine("All configuration values have been reset to their defaults.");
    }


    private static void InitializeData()
    {
        try
        {
            Console.WriteLine("Initializing data...");
            // Calling Initialization.Do with the appropriate objects
            Initialization.Do(s_dalAssignment, s_dalCall, s_dalConfig, s_dalVolunteer);
            Console.WriteLine("Initialization completed successfully.");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

    }

    private static void ShowAllData()
    {
        Console.WriteLine("Displaying all data:");
        Console.WriteLine("Volunteers:");
        foreach (var volunteer in s_dalVolunteer.ReadAll())
        {
            Console.WriteLine(volunteer);
        }
        Console.WriteLine("Calls:");
        foreach (var call in s_dalCall.ReadAll())
        {
            Console.WriteLine(call);
        }
        Console.WriteLine("Assignments:");
        foreach (var assignment in s_dalAssignment.ReadAll())
        {
            Console.WriteLine(assignment);
        }
    }

    private static void ResetDatabase()
    {
        Console.WriteLine("Resetting database...");
        s_dalVolunteer.DeleteAll();
        s_dalCall.DeleteAll();
        s_dalAssignment.DeleteAll();
        s_dalConfig.Reset();
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
                        ReadAllEntities();
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
            if (entityName == "Volunteer")
            {
                VolunteerImplementation V = new VolunteerImplementation();
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
                    Rol = Role.manager;

                Volunteer newVolunteer = new Volunteer
                {
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
                    V.Create(newVolunteer);
                    Console.WriteLine("Volunteer added successfully!");
                    isCreated = true;
                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }


            }

            if (entityName == "Call")
            {
                CallImplementation C = new CallImplementation();
                Console.WriteLine("enter id");
                int Id = int.Parse(Console.ReadLine());
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
                    Id = Id,
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
                    C.Create(newCall);
                    Console.WriteLine("Call added successfully!");
                    isCreated = true;
                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }


            }



            if (entityName == "Assignment")
            {
                AssignmentImplementation A = new AssignmentImplementation();
                Console.WriteLine("enter id");
                int Id = int.Parse(Console.ReadLine());
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
                    Status = TreatmentStatus.CanceledBymanager;
                if (st == "Expired")
                    Status = TreatmentStatus.Expired;
                Console.WriteLine("Enter the assignment entry time (format: yyyy-MM-dd HH:mm):");
                DateTime entryTime = DateTime.Parse(Console.ReadLine());
                Console.WriteLine("Enter the completion time (format: yyyy-MM-dd HH:mm, or press Enter to skip):");
                string completionTimeInput = Console.ReadLine();
                DateTime? completionTime = string.IsNullOrEmpty(completionTimeInput) ? null : DateTime.Parse(completionTimeInput);

                Assignment newAssignment = new Assignment
                {
                    Id = Id,
                    CallId = CallId,
                    VolunteerId = VolunteerId,
                    Status = Status,
                    EntryTime = entryTime,
                    CompletionTime = completionTime

                };
                try
                {
                    A.Create(newAssignment);
                    Console.WriteLine("Assignment added successfully!");
                    isCreated = true;
                }
                catch (InvalidOperationException ex)
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
            s_dalAssignment.DeleteAll();
            s_dalCall.DeleteAll();
            s_dalVolunteer.DeleteAll();
            Console.WriteLine("All Entities were deleted!");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

    }

    private static void DeleteEntity(string entityName)
    {
        try
        {
            int id;
            if (entityName == "Volunteer")
            {
                Console.WriteLine("enter the id of the volunteer");
                id = int.Parse(Console.ReadLine());
                s_dalVolunteer.Delete(id);
                Console.WriteLine("The volunteer was deleted");
            }

            if (entityName == "Call")
            {
                Console.WriteLine("enter the id of the call");
                id = int.Parse(Console.ReadLine());
                s_dalCall.Delete(id);
                Console.WriteLine("The call was deleted");
            }
            if (entityName == "Assignment")
            {
                Console.WriteLine("enter the id of the Assignment");
                id = int.Parse(Console.ReadLine());
                s_dalAssignment.Delete(id);
                Console.WriteLine("The Assignment was deleted");
            }
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }


    }

    private static void UpdateEntity(string entityName)
    {
        try
        {
            int id;
            if (entityName == "Call")
            {
                Console.WriteLine("Enter the ID of the call to update:");
                id = int.Parse(Console.ReadLine());
                var call = s_dalCall.Read(id);
                if (call == null)
                {
                    Console.WriteLine("Call not found.");
                    return;
                }
                Console.WriteLine("Current call details:");
                Console.WriteLine(call);

                // Requesting new values from the user
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

                // Update MaxCompletionTime if needed
                Console.WriteLine("Enter new max completion time (leave empty to keep current):");
                string newMaxCompletionTime = Console.ReadLine();
                if (DateTime.TryParse(newMaxCompletionTime, out DateTime maxCompletionTime))
                {
                    updatedCall = updatedCall with { MaxCompletionTime = maxCompletionTime };
                }

                // Finally, save the updated call
                s_dalCall.Update(updatedCall);
                Console.WriteLine("Call updated successfully.");
            }

            else if (entityName == "Assignment")
            {
                Console.WriteLine("Enter the ID of the assignment to update:");
                id = int.Parse(Console.ReadLine());
                var assignment = s_dalAssignment.Read(id);
                if (assignment == null)
                {
                    Console.WriteLine("Assignment not found.");
                    return;
                }
                Console.WriteLine("Current assignment details:");
                Console.WriteLine(assignment);

                // Requesting new values from the user
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

                // Finally, save the updated assignment
                s_dalAssignment.Update(updatedAssignment);
                Console.WriteLine("Assignment updated successfully.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }




    private static void ReadAllEntities()
    {
        try
        {
            Console.WriteLine("The Volunteers data:");
            s_dalAssignment.ReadAll();
            Console.WriteLine("The Assignments data:");
            s_dalCall.ReadAll();
            Console.WriteLine("The Calls data:");
            s_dalVolunteer.ReadAll();
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }



    private static void ReadEntity(string entityName)
    {
        try
        {
            int id;
            if (entityName == "Volunteer")
            {
                Console.WriteLine("enter the id of the volunteer");
                id = int.Parse(Console.ReadLine());
                s_dalVolunteer.Read(id);
            }

            if (entityName == "Call")
            {
                Console.WriteLine("enter the id of the call");
                id = int.Parse(Console.ReadLine());
                s_dalCall.Read(id);
            }
            if (entityName == "Assignment")
            {
                Console.WriteLine("enter the id of the Assignment");
                id = int.Parse(Console.ReadLine());
                s_dalAssignment.Read(id);
            }
        }
        catch (InvalidOperationException ex)
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
                // Display the main menu
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





