using Helpers;
using System;
using static DO.Enums;

namespace BO
{
    /// <summary>
    /// Represents a volunteer entity containing personal details, current address, 
    /// ongoing call, and various statistics related to their activities.
    /// </summary>
    public class Volunteer
    {
        public int Id { get; init; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string? Password { get; set; }
        public string? Address { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public Role Role { get; set; }
        public bool IsActive { get; set; }
        public double? MaxCallDistance { get; set; }
        public DistanceType DistanceType { get; set; }
        public int TotalCompletedCalls { get; set; }
        public int TotalCanceledCalls { get; set; }
        public int TotalExpiredCalls { get; set; }

        // ✅ שדה מתוקן: מ־CallProgress? ל־CallInProgress?
        public CallInProgress? CurrentCall { get; set; }

        public int? CurrentCallId { get; set; }

        public override string ToString() => Tools.ToStringProperty(this);
    }
}
