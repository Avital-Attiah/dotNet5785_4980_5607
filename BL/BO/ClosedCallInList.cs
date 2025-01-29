using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    /// <summary>
    /// Represents a closed call in a list, containing details such as ID, type, 
    /// full address, opening time, handling start time, and closure information.
    /// </summary>
    /// <param name="Id">Unique identifier for the call.</param>
    /// <param name="CallType">Type of the call (e.g., regular or emergency).</param>
    /// <param name="FullAddress">Full address of the call.</param>
    /// <param name="OpenTime">Time when the call was opened.</param>
    /// <param name="StartHandlingTime">Time when the call was assigned to a volunteer.</param>
    /// <param name="EndHandlingTime">Time when the call was closed (nullable if not closed).</param>
    /// <param name="ClosureType">Type of closure for the call (nullable, based on assignment).</param>
    public class ClosedCallInList
    {
        public int Id { get; init; }
        public CallType CallType { get; set; }
        public string FullAddress { get; set; }
        public DateTime OpenTime { get; set; }
        public DateTime StartHandlingTime { get; set; }
        public DateTime? EndHandlingTime { get; set; } // Nullable if not closed
        public ClosureType? ClosureType { get; set; }  // Nullable if closure type is not defined
    }
}
