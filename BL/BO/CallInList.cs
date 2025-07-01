using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    /// <summary>
    /// Data entity representing a "Call in List" containing details of the call, including a unique run identifier, type, address, open time, and more.
    /// </summary>
    /// <param name="Id">Unique identifier for the assignment entity. It will not be displayed in the UI. If no assignment exists, it will be null.</param>
    /// <param name="CallId">Unique identifier for the call.</param>
    /// <param name="CallType">The type of the call, based on the system's call type.</param>
    /// <param name="OpenTime">The time when the call was opened.</param>
    /// <param name="TimeRemaining">The remaining time for the call to be completed, calculated based on the maximum time minus the current system time.</param>
    /// <param name="LastAssignedVolunteerName">The name of the volunteer last assigned to the call. It will be null if no volunteer has been assigned.</param>
    /// <param name="TimeToComplete">The time taken to complete the treatment, calculated as the difference between the time the treatment ended and the call opening time. Relevant only for completed calls.</param>
    /// <param name="Status">The status of the call, calculated based on the assignment's end type, maximum time to complete, and the current system time.</param>
    /// <param name="AssignmentsCount">The total number of assignments for the current call, including how many times it was assigned, canceled, etc., until the current status.</param>
    public class CallInList
    {
        public int? Id { get; set; }
        public int CallId { get; set; }
        public CallType CallType { get; set; }
        public DateTime OpenTime { get; set; }
        public TimeSpan? TimeRemaining { get; set; }
        public string LastAssignedVolunteerName { get; set; }
        public TimeSpan? TimeToComplete { get; set; }
        public CallStatus Status { get; set; }
        public int AssignmentsCount { get; set; }
        public override string ToString() => Tools.ToStringProperty(this);
        public string CoordinateStatusMessage { get; set; }

    }
}
