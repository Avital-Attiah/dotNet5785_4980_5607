using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    /// <summary>
    /// Represents a volunteer in a list, containing details such as ID, name, 
    /// activity status, and statistics of handled, canceled, and expired calls.
    /// </summary>
    /// <param name="Id">Unique identifier for the volunteer.</param>
    /// <param name="FullName">Full name of the volunteer (first and last name).</param>
    /// <param name="IsActive">Indicates whether the volunteer is currently active.</param>
    /// <param name="TotalHandledCalls">Total number of calls handled by the volunteer.</param>
    /// <param name="TotalCanceledCalls">Total number of calls canceled by the volunteer.</param>
    /// <param name="TotalExpiredCalls">Total number of expired calls related to the volunteer.</param>
    /// <param name="CurrentCallId">Identifier of the call currently being handled by the volunteer (if any).</param>
    /// <param name="CurrentCallType">Type of the call currently being handled by the volunteer.</param>
    public class VolunteerInList
    {
        public int Id { get; init; }
        public string FullName { get; set; }
        public bool IsActive { get; set; }
        public int TotalHandledCalls { get; set; }
        public int TotalCanceledCalls { get; set; }
        public int TotalExpiredCalls { get; set; }
        public int? CurrentCallId { get; set; } // Nullable if no call is currently being handled
        public CallType CurrentCallType { get; set; } // ENUM, default to None if no call is being handled
    }
}
