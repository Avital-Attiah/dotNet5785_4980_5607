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
        lock (AdminManager.BlMutex) //stage 7
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

    internal static void PeriodicAssignmentsUpdates(DateTime oldClock, DateTime newClock)
    {
        var list = _dal.Assignment.ReadAll().ToList(); //  הפיכה לרשימה קונקרטית
        List<int> updatedAssignments = new(); //  רשימת מזהים שצריך לשלוח עליהם Notification

        foreach (var doAssignment in list)
        {
            if (doAssignment.CompletionTime.HasValue && newClock >= doAssignment.CompletionTime.Value)
            {
                // שלב 3: עדכון בתוך נעילה
                lock (AdminManager.BlMutex)
                {
                    _dal.Assignment.Update(doAssignment with { Status = DO.Enums.TreatmentStatus.Expired });
                }

                // שלב 4: נזכור את ה־ID כדי לעדכן אותו אחרי ה־lock
                updatedAssignments.Add(doAssignment.Id);
            }
        }

        
        foreach (var id in updatedAssignments)
        {
            Observers.NotifyItemUpdated(id);
        }

       
        if (updatedAssignments.Any())
        {
            Observers.NotifyListUpdated();
        }
    }

}
