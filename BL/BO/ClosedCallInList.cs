using Helpers;
using System;

namespace BO
{
    /// <summary>
    /// Represents a closed call in a list, containing details such as ID, type, 
    /// full address, opening time, handling start time, and closure information.
    /// </summary>
    public class ClosedCallInList
    {
        public int Id { get; init; }
        public CallType CallType { get; set; }
        public string FullAddress { get; set; }
        public DateTime OpenTime { get; set; }
        public DateTime StartHandlingTime { get; set; }
        public DateTime? EndHandlingTime { get; set; } // Nullable if not closed
        public ClosureType? ClosureType { get; set; }  // Nullable if closure type is not defined

        // Alias for compatibility:
        public DateTime? CloseDate
        {
            get => EndHandlingTime;
            set => EndHandlingTime = value;
        }

        public ClosureType? CloseStatus
        {
            get => ClosureType;
            set => ClosureType = value;
        }

        public override string ToString() => Tools.ToStringProperty(this);
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
