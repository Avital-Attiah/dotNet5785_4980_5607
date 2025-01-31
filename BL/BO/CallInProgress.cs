using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    /// <summary>
    /// Represents a call in progress, containing details about the call, 
    /// such as a unique identifier, type, address, opening time, and more.
    /// </summary>
    /// <param name="Id">Unique identifier for the call assignment.</param>
    /// <param name="CallId">Unique identifier for the specific call.</param>
    /// <param name="CallType">The type of the call (e.g., regular, emergency).</param>
    /// <param name="Description">A textual description of the call (optional).</param>
    /// <param name="FullAddress">The full address of the call location.</param>
    /// <param name="OpeningTime">The date and time the call was opened.</param>
    /// <param name="MaxCompletionTime">The maximum time allowed to complete the call (optional).</param>
    /// <param name="StartHandlingTime">The time the volunteer started handling the call.</param>
    /// <param name="DistanceFromVolunteer">The distance of the call from the volunteer handling it.</param>
    /// <param name="Status">The current status of the call (e.g., in progress, at risk).</param>

    public class CallInProgress
    {
        public int Id { get; init; }
        public int CallId { get; init; }
        public CallType CallType { get; set; } 
        public string? Description { get; set; } 
        public string FullAddress { get; set; }
        public DateTime OpeningTime { get; set; }
        public DateTime? MaxCompletionTime { get; set; } 
        public DateTime StartHandlingTime { get; set; }
        public double DistanceFromVolunteer { get; set; }
        public CallInProgressStatus Status { get; set; } 
    }

}
