namespace DalApi;
using DO;

public interface IConfig
{

    // Initial value for call ID
    int StartCallId { get; }

    // Auto-incrementing call ID
    int NextCallId { get; }

    // Initial value for assignment ID
    int StartAssignmentId { get; }

    // Auto-incrementing assignment ID
    int NextAssignmentId { get; }

    // System clock
    DateTime Clock { get; set; }

    // Risk time range
    TimeSpan RiskRange { get; set; }

    // Method to reset configuration variables to their initial values
    void Reset();
}
