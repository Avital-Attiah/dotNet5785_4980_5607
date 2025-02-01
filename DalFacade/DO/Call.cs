using System;
using static DO.Enums;

namespace DO
{
    /// <summary>
    /// A call entity containing the call details, including a unique auto-incremented ID, type, address, opening time, and more.
    /// </summary>
    /// <param name="Id">A unique identifier for the call.</param>
    /// <param name="CallType">The type of call (according to the system type).</param>
    /// <param name="FullAddress">The full address of the call location.</param>
    /// <param name="OpenTime">The time the call was opened.</param>
    /// <param name="isEmergency">A property to check whether the call is an emergency.</param>
    /// <param name="Description">A textual description of the call (default: null).</param>
    /// <param name="Latitude">The latitude of the call location (default: 0.0).</param>
    /// <param name="Longitude">The longitude of the call location (default: 0.0).</param>
    /// <param name="MaxCompletionTime">The maximum completion time for the call (if applicable, default: null).</param>
    public record Call
    (
        int Id,
        CallType CallType,
        string FullAddress,
        DateTime OpenTime,
        bool isEmergency = false,
        string? Description = null,
        double Latitude = 0.0,
        double Longitude = 0.0,
        DateTime? MaxCompletionTime = null
    )
    {


        // קונסטרקטור שלא מקבל ID - יוצר אוטומטית מספר רץ
        public Call()
            : this(0, CallType.EmotionalSupport, "", DateTime.Now, false, "", 0, 0, DateTime.Now) { }

        public override string ToString() =>
       $"Id: {Id}, Full Address: {FullAddress}, Latitude: {Latitude}, Longitude: {Longitude}, Time Open: {OpenTime}, " +
       $"Call Type: {CallType}, Verbal Description: {Description ?? "N/A"}, Max Time Finish: {MaxCompletionTime?.ToString() ?? "N/A"}";
    }
    
    }

   
