using static DO.Enums;

namespace DO
{
    /// <summary>
    /// An entity representing the assignment of a call to a volunteer, 
    /// linking a call to the volunteer who chose to handle it.
    /// </summary>
    /// <param name="Id">A unique identifier for the assignment.</param>
    /// <param name="CallId">The ID of the call the volunteer chose to handle.</param>
    /// <param name="VolunteerId">The volunteer's ID number.</param>
    /// <param name="EntryTime">The time the volunteer started handling the call.</param>
    /// <param name="CompletionTime">The actual time the call was completed.</param>
    /// <param name="Status">The type of completion status for the call.</param>
    public record Assignment
    (
        int Id,
        int CallId,
        int VolunteerId,
        DateTime EntryTime,
        DateTime? CompletionTime = null,
        TreatmentStatus? Status = null
    )
    {
        /// Default constructor - no parameters
        public Assignment() : this(0, 0, 0, DateTime.MinValue) { }
    }
}
