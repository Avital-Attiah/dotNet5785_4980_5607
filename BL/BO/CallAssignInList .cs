using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    namespace BO
    {
        /// <summary>
        /// Represents a call assignment in a list, containing details such as volunteer ID, volunteer name, 
        /// treatment start time, actual treatment end time, and the type of treatment completion.
        /// </summary>
        /// <param name="VolunteerId">ID of the assigned volunteer. Nullable if the assignment was artificially created for an unhandled call.</param>
        /// <param name="VolunteerName">Name of the assigned volunteer. Nullable if the assignment was artificially created.</param>
        /// <param name="StartTime">Time when the treatment started.</param>
        /// <param name="EndTime">Actual end time of the treatment. Nullable if the treatment has not yet been completed.</param>
        /// <param name="CompletionType">Type of treatment completion. Nullable if the treatment has not yet been completed.</param>
        public class CallAssignInList
        {
            public int? VolunteerId { get; set; } // Nullable if created artificially
            public string? VolunteerName { get; set; } // Nullable if no volunteer is assigned
            public DateTime StartTime { get; set; } // Time when the treatment started
            public DateTime? EndTime { get; set; } // Nullable if the treatment is not completed
            public CompletionType? CompletionType { get; set; } // Nullable if treatment is not completed
        }
    }
}
