
namespace Dal;
using DalApi;
sealed internal class DalList : IDal
{
    //public IVolunteer Volunteer => throw new NotImplementedException();
    public static IDal Instance { get; } = new DalList();
    private DalList() { }

    public IVolunteer Volunteer { get; } = new VolunteerImplementation();
    public ICall Call { get; } = new CallImplementation();
    public IConfig Config { get; } = new ConfigImplementation();
    public IAssignment Assignment { get; } = new AssignmentImplementation();
    //public ICall Call => throw new NotImplementedException();

    //public IConfig Config => throw new NotImplementedException();

    //public IAssignment Assignment => throw new NotImplementedException();
    public void ResetDB()
    {
        Volunteer.DeleteAll();
        Call.DeleteAll();
        Assignment.DeleteAll(); 
        Config.Reset();
    }

}
