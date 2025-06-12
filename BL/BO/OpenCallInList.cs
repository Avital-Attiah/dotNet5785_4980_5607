using Helpers;
using System;

namespace BO
{
    public class OpenCallInList
    {
        public int Id { get; init; }
        public CallType CallType { get; set; }
        public string? Description { get; set; }
        public string FullAddress { get; set; }
        public DateTime OpenTime { get; set; }
        public DateTime? MaxCompletionTime { get; set; }
        public double DistanceFromVolunteer { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public override string ToString() => Tools.ToStringProperty(this);
    }
}
