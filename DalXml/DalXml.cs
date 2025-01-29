using DalApi;
using System.Diagnostics;

namespace Dal;

sealed internal class DalXml : IDal
{
    public static IDal Instance { get; } = new DalXml(); // Singleton instance of DalXml
    private DalXml() { } // Private constructor to prevent external instantiation

    public IVolunteer Volunteer { get; } = new VolunteerImplementation(); // Provides access to volunteer operations
    public ICall Call { get; } = new CallImplementation(); // Provides access to call operations
    public IConfig Config { get; } = new ConfigImplementation(); // Provides access to configuration settings
    public IAssignment Assignment { get; } = new AssignmentImplementation(); // Provides access to assignment operations

    public void ResetDB()
    {
        Volunteer.DeleteAll(); // Deletes all volunteer records
        Call.DeleteAll(); // Deletes all call records
        Assignment.DeleteAll(); // Deletes all assignment records
        Config.Reset(); // Resets the configuration settings to their initial values
    }
}
