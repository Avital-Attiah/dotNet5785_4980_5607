using BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlApi
{
    public interface ICall
    {
        int[] GetCallCounts();
        public IEnumerable<BO.CallInList> GetCallsList(
        CallInListFieldSor? filterByField = null,
        object? Value = null,
        CallInListFieldSor? sortByField = null);
        BO.Call Read(int callId);
        void UpdateCallDetails(BO.Call call);
        void DeleteCall(int callId);
        void Create(BO.Call call);
        public IEnumerable<BO.ClosedCallInList> GetClosedCallsByVolunteer(
        int volunteerId,
        CallType? callTypeFilter = null,
        ClosedCallInListFields? sortField = null);
        public IEnumerable<BO.OpenCallInList> GetOpenCalls(
        int volunteerId,
        CallType? callTypeFilter = null,
        OpenCallInListFields? sortField = null);
        void FinishCall(int volunteerId, int callId);
        void CancellationOfTreatment(int CancellerId, int assignmentId);
        void SelectCall(int volunteerId, int callId);
    }
}
