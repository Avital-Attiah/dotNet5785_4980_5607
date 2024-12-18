using DalApi;
namespace Dal;

internal class ConfigImplementation : IConfig
{
    public DateTime Clock
    {
        get => Config.Clock;
        set => Config.Clock = value;
    }

    public int StartCallId
    {
        get => Config.startCallId;
    }

    public int NextCallId
    {
        get => Config.NextCallId;
    }

    public int StartAssignmentId
    {
        get => Config.startAssignmentId;
    }

    public int NextAssignmentId
    {
        get => Config.NextAssignmentId;
    }

    public TimeSpan RiskRange
    {
        get => Config.RiskRange;
        set => Config.RiskRange = value;
    }

    public void Reset()
    {
        Config.Reset();
    }
}
