using BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers;


namespace BlApi
{

    // Defines an interface for handling call-related operations
    public interface ICall : IObservable //stage 5 הרחבת ממשק
    {
        // Retrieves an array of counts for different call statuses
        int[] GetCallCounts();

        // Retrieves a filtered and/or sorted list of calls
        IEnumerable<BO.CallInList> GetCallsList(BO.CallInListFieldSor? filterByField = null, object? value = null, BO.CallInListFieldSor? sortByField = null);

        // Reads and returns the details of a specific call by its ID
        BO.Call? Read(int callId);

        // Updates the details of an existing call
        void UpdateCallDetails(BO.Call updateCallObj);

        // Deletes a call based on its ID
        void DeleteCall(int callId);

        // Creates a new call record
        void Create(BO.Call boCall);

        // Retrieves a list of closed calls handled by a specific volunteer
        IEnumerable<BO.ClosedCallInList> GetClosedCallsByVolunteer(int volunteerId, BO.CallType? callTypeFilter = null, BO.ClosedCallInListFields? sortField = null);

        // Retrieves a list of open calls available for a specific volunteer
        IEnumerable<BO.OpenCallInList> GetOpenCalls(int volunteerId, BO.CallType? callTypeFilter = null, BO.OpenCallInListFields? sortField = null);

        // Marks a call as completed by a volunteer
        void FinishCall(int volunteerId, int assignmentId);

        // Cancels a call treatment, specifying the canceller and assignment ID
        void CancellationOfTreatment(int CancellerId, int assignmentId);

        // Assigns a volunteer to handle a specific call
        void SelectCall(int volunteerId, int callId);
    }

   
}
