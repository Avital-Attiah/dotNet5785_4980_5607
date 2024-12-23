using DalApi;

namespace Dal;

sealed public class DalXml : IDal
{
    public IVolunteer Volunteer { get; } = new VolunteerImplementation();
    public ICall Call { get; } = new CallImplementation();
    public IConfig Config { get; } = new ConfigImplementation();
    public IAssignment Assignment { get; } = new AssignmentImplementation();

    public void ResetDB()
    {
        Volunteer.DeleteAll();
        Call.DeleteAll();
        Assignment.DeleteAll();
        Config.Reset();
    }
}
