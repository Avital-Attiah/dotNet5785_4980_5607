using DalApi;
namespace Dal;

internal class ConfigImplementation : IConfig
{
    public DateTime Clock
    {
        get => Config.Clock; // Gets the system clock time
        set => Config.Clock = value; // Sets the system clock time
    }

    public int StartCallId
    {
        get => Config.startCallId; // Gets the initial call ID value
    }

    public int NextCallId
    {
        get => Config.NextCallId; // Gets the next call ID value
    }

    public int StartAssignmentId
    {
        get => Config.startAssignmentId; // Gets the initial assignment ID value
    }

    public int NextAssignmentId
    {
        get => Config.NextAssignmentId; // Gets the next assignment ID value
    }

    public TimeSpan RiskRange
    {
        get => Config.RiskRange; // Gets the risk time range
        set => Config.RiskRange = value; // Sets the risk time range
    }

    public void Reset()
    {
        Config.Reset(); // Resets all configuration values to their initial state
    }
}
