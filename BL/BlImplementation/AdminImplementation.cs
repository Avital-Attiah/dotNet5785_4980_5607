namespace BlImplementation;
using BlApi;
using Helpers;
using System;

internal class AdminImplementation : IAdmin
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    DateTime IAdmin.GetClock()
    {
        return _dal.Config.Clock;
    }
    public void UpdateClock(BO.TimeUnit timeUnit)
    {
        switch (timeUnit)
        {
            case BO.TimeUnit.MINUTE:
                ClockManager.UpdateClock(ClockManager.Now.AddMinutes(1));
                break;
            case BO.TimeUnit.HOUR:
                ClockManager.UpdateClock(ClockManager.Now.AddHours(1));
                break;
            case BO.TimeUnit.DAY:
                ClockManager.UpdateClock(ClockManager.Now.AddDays(1));
                break;
            case BO.TimeUnit.MONTH:
                ClockManager.UpdateClock(ClockManager.Now.AddMonths(1));
                break;
            case BO.TimeUnit.YEAR:
                ClockManager.UpdateClock(ClockManager.Now.AddYears(1));
                break;
            default:

                break;
        }
        return;
    }

    TimeSpan IAdmin.GetRiskRange()
    {
        return _dal.Config.RiskRange;
    }

    void IAdmin.SetRiskRange(TimeSpan riskRange)
    {
        _dal.Config.RiskRange = riskRange;
    }

    void IAdmin.ResetDB()
    {
        _dal.ResetDB();
        ClockManager.UpdateClock(ClockManager.Now);

    }
    void IAdmin.InitializeDB()
    {
        DalTest.Initialization.Do();
        ClockManager.UpdateClock(ClockManager.Now);
    }
}