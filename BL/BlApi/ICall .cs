using BO; // Imports the Business Objects (BO) namespace

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlApi
{
    // Defines an interface for handling call-related operations
    public interface ICall
    {
        // Retrieves an array of counts for different call statuses
        int[] GetCallCounts();

        // Retrieves a filtered and/or sorted list of calls
        public IEnumerable<BO.CallInList> GetCallsList(
            CallInListFieldSor? filterByField = null, // Optional filter criteria
            object? Value = null, // Value for filtering
            CallInListFieldSor? sortByField = null // Optional sorting criteria
        );

        // Reads and returns the details of a specific call by its ID
        BO.Call Read(int callId);

        // Updates the details of an existing call
        void UpdateCallDetails(BO.Call call);

        // Deletes a call based on its ID
        void DeleteCall(int callId);

        // Creates a new call record
        void Create(BO.Call call);

        // Retrieves a list of closed calls handled by a specific volunteer
        public IEnumerable<BO.ClosedCallInList> GetClosedCallsByVolunteer(
            int volunteerId, // ID of the volunteer
            CallType? callTypeFilter = null, // Optional filter by call type
            ClosedCallInListFields? sortField = null // Optional sorting field
        );

        // Retrieves a list of open calls available for a specific volunteer
        public IEnumerable<BO.OpenCallInList> GetOpenCalls(
            int volunteerId, // ID of the volunteer
            CallType? callTypeFilter = null, // Optional filter by call type
            OpenCallInListFields? sortField = null // Optional sorting field
        );

        // Marks a call as completed by a volunteer
        void FinishCall(int volunteerId, int callId);

        // Cancels a call treatment, specifying the canceller and assignment ID
        void CancellationOfTreatment(int CancellerId, int assignmentId);

        // Assigns a volunteer to handle a specific call
        void SelectCall(int volunteerId, int callId);
    }
}
