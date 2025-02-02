using DalApi;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;


namespace Helpers;

internal static class CallManager
{
    private static IDal s_dal = Factory.Get;
    internal static void UpdateExpiredOpenCalls()
    {
        var list = s_dal.Call.ReadAll().ToList();
        foreach (var doCall in list)
        {
            if (doCall.MaxCompletionTime <= ClockManager.Now)
            {
                var assignment = s_dal.Assignment.Read(a => a.CallId == doCall.Id);
                if (assignment == null)
                {
                    DO.Assignment doAssignment =
                     new(doCall.Id, 0,0, ClockManager.Now, ClockManager.Now, DO.Enums.TreatmentStatus.Expired);
                    s_dal.Assignment.Create(doAssignment);
                }
                else
                {
                    DO.Assignment updatedAssignment = assignment with { CompletionTime = ClockManager.Now, Status = DO.Enums.TreatmentStatus.Expired };
                    s_dal.Assignment.Update(updatedAssignment);
                }
            }
        }

    }

    internal static BO.CallStatus GetStatusCall(int id)
    {
        DO.Call? doCall = s_dal.Call.Read(id)!;
        DO.Assignment? doAssignment = s_dal.Assignment.Read(a => a.CallId == id);
        DateTime now = ClockManager.Now;

        TimeSpan? timeDifference = doCall.MaxCompletionTime.HasValue
            ? doCall.MaxCompletionTime.Value - now
            : (TimeSpan?)null;

        if (doAssignment == null)
        {
            if (doCall.MaxCompletionTime <= now)
            {
                return BO.CallStatus.Expired;
            }

            if (timeDifference.HasValue && timeDifference <= s_dal.Config.RiskRange)
            {
                return BO.CallStatus.OpenAtRisk;
            }

            return BO.CallStatus.Open;
        }

        if (timeDifference.HasValue)
        {
            if (timeDifference <= TimeSpan.Zero)
            {
                return BO.CallStatus.Expired;
            }

            if (timeDifference <= s_dal.Config.RiskRange)
            {
                return BO.CallStatus.InProgressAtRisk;
            }
        }

        return BO.CallStatus.InProgress;
    }


    internal static List<BO.CallAssignInList> GetAssignmentsHistory(int id)
    {
        return s_dal.Assignment.ReadAll(a => a.CallId == id)
            .Select(item => new BO.CallAssignInList
            {
                VolunteerId = item.VolunteerId,
                VolunteerName = s_dal.Volunteer.Read(item.VolunteerId).FullName,
                StartTime = item.EntryTime,
                EndTime = item.CompletionTime,
                CompletionType = (BO.CompletionType?)item.Status
            })
            .ToList();
    }

    #region check address
    /// <summary>
    /// Retrieves the geographical coordinates (latitude and longitude) for a given address.
    /// </summary>
    /// <param name="address">The address to get the coordinates for.</param>
    /// <returns>A tuple containing the latitude and longitude of the address.</returns>
    /// <exception cref="BO.BlValidationException">Thrown when the address is invalid.</exception>
    public static (double Latitude, double Longitude) GetCoordinates(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
        {
            throw new BO.BlValidationException("The address is invalid.");
        }

        string url = $"https://geocode.maps.co/search?q={Uri.EscapeDataString(address)}&api_key=679a8da6c01a6853187846vomb04142";

        try
        {
            using (WebClient client = new WebClient())
            {
                string response = client.DownloadString(url);

                var result = JsonSerializer.Deserialize<GeocodeResponse[]>(response);

                if (result == null || result.Length == 0)
                {
                    throw new BO.BlValidationException("The address is invalid.");
                }

                double latitude = double.Parse(result[0].Latitude);
                double longitude = double.Parse(result[0].Longitude);

                return (latitude, longitude);
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Error retrieving coordinates" + ex.Message);
        }
    }

    /// <summary>
    /// Represents the structure of a geocoding response.
    /// </summary>
    private class GeocodeResponse
    {
        [JsonPropertyName("lat")]
        public string Latitude { get; set; } // Latitude as string

        [JsonPropertyName("lon")]
        public string Longitude { get; set; } // Longitude as string

        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; } // Full address representation
    }
    #endregion


    public static double CalculateHaversineDistance(double lat1, double lon1, double lat2, double lon2)
    {
        double EarthRadiusKm = 6371.0;
        double dLat = DegreesToRadians(lat2 - lat1);
        double dLon = DegreesToRadians(lon2 - lon1);

        double a = Math.Pow(Math.Sin(dLat / 2), 2) +
                   Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                   Math.Pow(Math.Sin(dLon / 2), 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return EarthRadiusKm * c;
    }

    private static double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180;
    }

    public static double GetDistanceBetweenAddresses(string address1, string address2)
    {
        try
        {
            var coords1 = GetCoordinates(address1);
            var coords2 = GetCoordinates(address2);

            if (coords1 == null || coords2 == null)
            {
                throw new Exception("Unable to calculate distance - one or two of the addresses is invalid");
            }

            return CalculateHaversineDistance(coords1.Item1, coords1.Item2, coords2.Item1, coords2.Item2);
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public static bool IsValidNumber(string input)
    {
        var numberRegex = new Regex(@"^\d+$");
        return numberRegex.IsMatch(input);
    }

    public static void ValidateCall(BO.Call boCall, bool isUpdate = false)
    {
        // Validation - TimeOpen and MaxTimeFinishCall
        if (boCall.MaxCompletionTime.HasValue && boCall.OpenTime >= boCall.MaxCompletionTime)
        {
            throw new BO.BlValidationException("MaxTimeFinishCall must be later than TimeOpen.");
        }

        // Validation - FullAddressOfCall
        if (!string.IsNullOrEmpty(boCall.FullAddress))
        {
            try
            {
                var (latitude, longitude) = GetCoordinates(boCall.FullAddress);
                boCall.Latitude = latitude;
                boCall.Longitude = longitude;
            }
            catch (BO.BlValidationException)
            {
                throw new BO.BlValidationException($"Invalid address: {boCall.FullAddress}");
            }
        }
        else
        {
            throw new BO.BlValidationException("Full address of the call is required.");
        }

        // Validation - Description
        if (string.IsNullOrEmpty(boCall.Description))
        {
            throw new BO.BlValidationException("Verbal description is required.");
        }

        // Validation - CallType
        if (!VolunteerManager.IsValidEnum<BO.CallType>((int)boCall.CallType))
        {
            throw new BO.BlValidationException($"Invalid TypeCall value: {boCall.CallType}");
        }
    }

    public static DO.Call updateIfNeededDoCall(DO.Call doCall, BO.Call boCAll)
    {
        var copyDoCall = doCall;

        if (doCall.OpenTime != boCAll.OpenTime || doCall.MaxCompletionTime != boCAll.MaxCompletionTime)
        {
            copyDoCall = copyDoCall with { OpenTime = boCAll.OpenTime, MaxCompletionTime = boCAll.MaxCompletionTime };
        }

        if (doCall.FullAddress != boCAll.FullAddress)
        {
            copyDoCall = copyDoCall with { FullAddress = boCAll.FullAddress, Latitude = boCAll.Latitude, Longitude = boCAll.Longitude };
        }

        if (doCall.Description != boCAll.Description)
        {
            copyDoCall = copyDoCall with { Description = boCAll.Description };
        }

        if (doCall.CallType != (DO.Enums.CallType)boCAll.CallType)
        {
            copyDoCall = copyDoCall with { CallType = (DO.Enums.CallType)boCAll.CallType };
        }
        return copyDoCall;
    }

}