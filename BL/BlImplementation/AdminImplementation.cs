namespace BlImplementation; // Defines the namespace for the Business Logic implementation

using BlApi; // Imports the Business Logic API interface
using Helpers; // Imports helper utilities
using System;

internal class AdminImplementation : IAdmin // Implements the IAdmin interface
{
    // Reference to the Data Access Layer (DAL) instance
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    // Retrieves the current clock time from the DAL configuration
    DateTime IAdmin.GetClock()
    {
        return _dal.Config.Clock;
    }

    // Updates the system clock based on the specified time unit
    public void UpdateClock(BO.TimeUnit timeUnit)
    {
        switch (timeUnit)
        {
            case BO.TimeUnit.MINUTE:
                ClockManager.UpdateClock(ClockManager.Now.AddMinutes(1)); // Adds 1 minute
                break;
            case BO.TimeUnit.HOUR:
                ClockManager.UpdateClock(ClockManager.Now.AddHours(1)); // Adds 1 hour
                break;
            case BO.TimeUnit.DAY:
                ClockManager.UpdateClock(ClockManager.Now.AddDays(1)); // Adds 1 day
                break;
            case BO.TimeUnit.MONTH:
                ClockManager.UpdateClock(ClockManager.Now.AddMonths(1)); // Adds 1 month
                break;
            case BO.TimeUnit.YEAR:
                ClockManager.UpdateClock(ClockManager.Now.AddYears(1)); // Adds 1 year
                break;
            default:
                break;
        }
    }

    // Retrieves the configured risk range from the DAL
    TimeSpan IAdmin.GetRiskRange()
    {
        return _dal.Config.RiskRange;
    }

    // Sets a new risk range in the DAL configuration
    void IAdmin.SetRiskRange(TimeSpan riskRange)
    {
        _dal.Config.RiskRange = riskRange;
    }

    // Resets the database and updates the clock to the current time
    void IAdmin.ResetDB()
    {
        _dal.ResetDB(); // Calls the DAL method to reset the database
        ClockManager.UpdateClock(ClockManager.Now); // Updates the system clock
    }

    // Initializes the database with test data and updates the clock
    void IAdmin.InitializeDB()
    {
        DalTest.Initialization.Do(); // Calls the DAL test initialization
        ClockManager.UpdateClock(ClockManager.Now); // Updates the system clock
    }
}
