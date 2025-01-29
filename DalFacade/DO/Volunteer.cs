using static DO.Enums;

namespace DO
{
    /// <summary>
    /// The Volunteer entity, containing volunteer details, including a unique ID and additional information about the volunteer's activities.
    /// </summary>
    /// <param name="Id">Volunteer ID, a unique identifier for the volunteer.</param>
    /// <param name="FullName">The volunteer's full name (first and last name).</param>
    /// <param name="Phone">The volunteer's valid mobile phone number.</param>
    /// <param name="Email">The volunteer's email address.</param>
    /// <param name="Password">The volunteer's password, which can be null initially.</param>
    /// <param name="CurrentAddress">The volunteer's full address.</param>
    /// <param name="Latitude">The volunteer's latitude, calculated based on the address.</param>
    /// <param name="Longitude">The volunteer's longitude, calculated based on the address.</param>
    /// <param name="Role">The volunteer's role (Manager or Volunteer).</param>
    /// <param name="IsActive">Indicates whether the volunteer is currently active in the organization.</param>
    /// <param name="MaxDistance">The maximum distance within which the volunteer can accept calls.</param>
    /// <param name="DistanceType">The type of distance measurement (Air, Walking, Driving).</param>
    public record Volunteer
    (
        int Id,
        string FullName,
        string Phone,
        string Email,
        string? Password = null,
        string? CurrentAddress = null,
        double? Latitude = null,
        double? Longitude = null,
        Role Role = default,
        bool IsActive = true,
        double? MaxDistance = null,
        DistanceType DistanceType = default
    )
    {
        // Empty (default) constructor - required for any record entity.
        public Volunteer() : this(0, "", "", "") { }
    }
}
