using DalApi;

namespace Helpers;

internal static class AssignmentManager
{
    internal static ObserverManager Observers = new(); //stage 5 
    private static IDal _dal = Factory.Get;

    /// <summary>
    /// Returns the history of assignments for a specific call.
    /// </summary>
    /// <param name="id">The ID of the call.</param>
    /// <returns>List of assignment details for the given call.</returns>
    internal static List<BO.CallAssignInList> GetAssignmentsHistory(int id)
    {
        return _dal.Assignment.ReadAll(a => a.CallId == id)
            .Select(item => new BO.CallAssignInList
            {
                VolunteerId = item.VolunteerId,
                VolunteerName = _dal.Volunteer.Read(item.VolunteerId).FullName,
                StartTime = item.EntryTime,
                EndTime = item.CompletionTime,
                CompletionType = (BO.CompletionType?)(BO.ClosureType?)item.Status
            })
            .ToList();
    }
}