using Dal;
using DalApi;
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
        Console.WriteLine("\n--- Configuration Submenu ---");
        Console.WriteLine("0. Exit");
        Console.WriteLine("1. Advance System Clock by One Minute");
        Console.WriteLine("2. Advance System Clock by One Hour");
        Console.WriteLine("3. Show Current Time");
        Console.WriteLine("4. Set Configuration Value");
        Console.WriteLine("5. Display Configuration Value");
        Console.WriteLine("6. Reset Configuration");
    }

    private static void InitializeData()
    {
        Console.WriteLine("Initializing data...");
        // Calling Initialization.Do with the appropriate objects
        Initialization.Do(s_dalAssignment, s_dalCall, s_dalConfig, s_dalVolunteer);
        Console.WriteLine("Initialization completed successfully.");
    }

    private static void ShowAllData()
    {
        Console.WriteLine("Displaying all data:");
        // Displaying data for each entity
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
        bool exit = false;

        while (!exit)
        {
            try
            {
                // הצגת תפריט CRUD עבור הישות
                Console.WriteLine("Menu of"+entityName+":");
                Console.WriteLine("0.  Exit");
                Console.WriteLine("1. Create");
                Console.WriteLine("2. Read");
                Console.WriteLine("3.ReadAll");
                Console.WriteLine("4. Update");
                Console.WriteLine("5.Delete");
                Console.WriteLine("6.DeleteAll");
                Console.Write("select option:");
                string choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "0":
                            exit = true;
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
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An exception occurred: {ex.Message}");
            }
        }
    }


    private static void CreateEntity(string entityName)
    {
        throw new NotImplementedException();
    }
  

    private static void DeleteAllEntities(string entityName)
    {
        throw new NotImplementedException();
    }

    private static void DeleteEntity(string entityName)
    {
        throw new NotImplementedException();
    }

    private static void UpdateEntity(string entityName)
    {
        throw new NotImplementedException();
    }

    private static void ReadAllEntities(string entityName)
    {
        throw new NotImplementedException();
    }

    private static void ReadEntity(string entityName)
    {
        throw new NotImplementedException();
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





