using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DO.Enums;

namespace BO
{

    /// <summary>
    /// Represents a volunteer entity containing personal details, current address, 
    /// ongoing call, and various statistics related to their activities.
    /// </summary>
    /// <param name="Id">Unique identifier for the volunteer (cannot be null).</param>
    /// <param name="FullName">Full name of the volunteer (cannot be null).</param>
    /// <param name="Phone">Volunteer’s mobile phone number (cannot be null).</param>
    /// <param name="Email">Volunteer’s email address (cannot be null).</param>
    /// <param name="Password">Password for the volunteer (optional, can be null).</param>
    /// <param name="Address">Current full address of the volunteer (optional, can be null).</param>
    /// <param name="Latitude">Latitude of the volunteer's current address (optional, can be null).</param>
    /// <param name="Longitude">Longitude of the volunteer's current address (optional, can be null).</param>
    /// <param name="Role">Role of the volunteer (cannot be null).</param>
    /// <param name="IsActive">Indicates if the volunteer is active or inactive (cannot be null).</param>
    /// <param name="MaxCallDistance">Maximum distance for receiving a call (optional, can be null).</param>
    /// <param name="DistanceType">Type of distance calculation (cannot be null).</param>
    /// <param name="TotalCompletedCalls">Number of calls successfully completed by the volunteer (cannot be null).</param>
    /// <param name="TotalCanceledCalls">Number of calls canceled by the volunteer (cannot be null).</param>
    /// <param name="TotalExpiredCalls">Number of calls that expired while assigned to the volunteer (cannot be null).</param>
    /// <param name="CurrentCall">The current call being handled by the volunteer (optional, can be null).</param>
    public class Volunteer
    {
        int Id { get; init; }
        string FullName { get; set; }
        string Phone { get; set; }
        string Email { get; set; }
        string? Password { get; set; }
        string? Address { get; set; }
        double? Latitude { get; set; }
        double? Longitude { get; set; }
        VolunteerRole Role { get; set; }
        bool IsActive { get; set; }
        double? MaxCallDistance { get; set; }
        DistanceType DistanceType { get; set; }
        int TotalCompletedCalls { get; set; }
        int TotalCanceledCalls { get; set; }
        int TotalExpiredCalls { get; set; }
        CallInProgress? CurrentCall { get; set; }
    };

  
}
