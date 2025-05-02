using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    /// <summary>
    /// Represents an open call in a list, containing details such as ID, type, description, 
    /// full address, opening time, maximum completion time, and distance from the volunteer.
    /// </summary>
    /// <param name="Id">Unique identifier for the call.</param>
    /// <param name="CallType">Type of the call (e.g., regular or emergency).</param>
    /// <param name="Description">Optional description of the call.</param>
    /// <param name="FullAddress">Full address of the call.</param>
    /// <param name="OpenTime">Time when the call was opened.</param>
    /// <param name="MaxCompletionTime">Optional maximum time by which the call should be completed.</param>
    /// <param name="DistanceFromVolunteer">Calculated distance from the current location of the volunteer.</param>
    public class OpenCallInList
    {
        public int Id { get; init; }
        public CallType CallType { get; set; }
        public string? Description { get; set; } // Nullable because the description might not exist
        public string FullAddress { get; set; }
        public DateTime OpenTime { get; set; }
        public DateTime? MaxCompletionTime { get; set; } // Nullable if no maximum completion time is set
        public double DistanceFromVolunteer { get; set; } // Calculated value
        public override string ToString() => Tools.ToStringProperty(this);
    }
}
