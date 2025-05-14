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

    public class CallImplementation : ICall
    {
        private List<BO.CallInList> calls = new List<BO.CallInList>();

        public IEnumerable<BO.CallInList> GetCallsList(BO.CallInListFieldSor? filterByField = null, object? value = null, BO.CallInListFieldSor? sortByField = null)
        {
            var query = calls.AsEnumerable();

            if (filterByField.HasValue && value != null)
            {
                switch (filterByField.Value)
                {
                    case BO.CallInListFieldSor.CallId:
                        query = query.Where(c => c.CallId == (int)value);
                        break;
                        // Add more filtering cases as needed
                }
            }

            if (sortByField.HasValue)
            {
                switch (sortByField.Value)
                {
                    case BO.CallInListFieldSor.TimeOpen:
                        query = query.OrderBy(c => c.OpenTime);
                        break;
                    case BO.CallInListFieldSor.Id:
                        query = query.OrderBy(c => c.Id);
                        break;
                    case BO.CallInListFieldSor.CallId:
                        query = query.OrderBy(c => c.CallId);
                        break;
                        // Add other sorting cases as needed
                }
            }

            return query;
        }

        // Implement other interface methods as needed...

        public int[] GetCallCounts() => throw new NotImplementedException();
        public BO.Call? Read(int callId) => throw new NotImplementedException();
        public void UpdateCallDetails(BO.Call updateCallObj) => throw new NotImplementedException();
        public void DeleteCall(int callId) => throw new NotImplementedException();
        public void Create(BO.Call boCall) => throw new NotImplementedException();
        public IEnumerable<BO.ClosedCallInList> GetClosedCallsByVolunteer(int volunteerId, BO.CallType? callTypeFilter = null, BO.ClosedCallInListFields? sortField = null) => throw new NotImplementedException();
        public IEnumerable<BO.OpenCallInList> GetOpenCalls(int volunteerId, BO.CallType? callTypeFilter = null, BO.OpenCallInListFields? sortField = null) => throw new NotImplementedException();
        public void FinishCall(int volunteerId, int assignmentId) => throw new NotImplementedException();
        public void CancellationOfTreatment(int CancellerId, int assignmentId) => throw new NotImplementedException();
        public void SelectCall(int volunteerId, int callId) => throw new NotImplementedException();
    }
}
