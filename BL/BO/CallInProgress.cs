using Helpers;
using System;

namespace BO
{
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

        // ✅ מתוקן: היה CallStatus
        public CallProgress Status { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public override string ToString() => Tools.ToStringProperty(this);
    }
}
