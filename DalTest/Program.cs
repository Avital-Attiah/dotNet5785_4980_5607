using Dal;
using DalApi;

namespace DalTest;

internal class Program
{
    private static IVolunteer? s_dalVolunteer = new VolunteerImplementation(); //stage 1
    private static ICall? s_dalCall = new CallImplementation(); //stage 1
    private static IAssignment? s_dalAssignment = new AssignmentImplementation(); //stage 1
    private static IConfig? s_dalConfig = new ConfigImplementation();

    private enum MainMenuOption
    {
        Exit,
        ManageVolunteers,
        ManageCalls,
        ManageAssignments,
        ManageConfig,
        InitializeDatabase,
        ResetDatabase
    }

    // ENUM לתתי-תפריטים עבור CRUD
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
    static void Main(string[] args)
    {
         
    }
   


}


