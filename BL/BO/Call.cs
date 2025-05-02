using BO;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class Call
    {
        /// <summary>
        /// Represents a logical data entity for a call, including details such as 
        /// its unique identifier, type, address, description, status, and a list of assignments.
        /// </summary>
        /// <param name="Id">Unique identifier for the call (cannot be null).</param>
        /// <param name="CallType">Type of the call (e.g., Regular or Emergency) (cannot be null).</param>
        /// <param name="Description">A descriptive text about the call (optional, can be null).</param>
        /// <param name="FullAddress">Full address of the call (cannot be null).</param>
        /// <param name="Latitude">Latitude of the call's location (cannot be null).</param>
        /// <param name="Longitude">Longitude of the call's location (cannot be null).</param>
        /// <param name="OpenTime">The time the call was opened (cannot be null).</param>
        /// <param name="MaxCompletionTime">The maximum time allowed for the call's completion (optional, can be null).</param>
        /// <param name="Status">The current status of the call (cannot be null).</param>
        /// <param name="Assignments">A list of call assignments related to this call (optional, can be null).</param>

        public int Id { get; init; } // Non-nullable
        public CallType CallType { get; set; } // ENUM, non-nullable
        public string? Description { get; set; } // Nullable
        public string FullAddress { get; set; } // Non-nullable
        public double Latitude { get; set; } // Non-nullable
        public double Longitude { get; set; } // Non-nullable
        public DateTime OpenTime { get; init; } // Non-nullable, set once
        public DateTime? MaxCompletionTime { get; set; } // Nullable
        public CallStatus Status { get; set; } // ENUM, non-nullable
        public List<CallAssignInList>? ListAssignments { get; set; } // Nullable if no assignments exist
        public override string ToString() => Tools.ToStringProperty(this);
    }
}
