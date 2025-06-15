using DalApi;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using DO;
using System.Globalization;


namespace Helpers
{
    // Static class responsible for managing calls, assignments, and validations
    internal static class CallManager
    {
        internal static ObserverManager Observers = new(); //stage 5 
        private static IDal s_dal = Factory.Get; // Data access layer

        /// <summary>
        /// Updates the status of open calls that have expired by comparing their completion time to the current time.
        /// </summary>
        internal static void UpdateExpiredOpenCalls()
        {
            var list = s_dal.Call.ReadAll().ToList(); // Get all calls
         
            foreach (var doCall in list)
            {

                if (doCall.MaxCompletionTime <= AdminManager.Now)
                {
                    var assignment = s_dal.Assignment.Read(a => a.CallId == doCall.Id);

                    if (assignment == null)
                    {
                        // If no assignment exists, create a new expired assignment
                        DO.Assignment doAssignment =
                         new(0,doCall.Id, 0, AdminManager.Now, AdminManager.Now, DO.Enums.TreatmentStatus.Expired);
                        s_dal.Assignment.Create(doAssignment);
                    }
                    else
                    {
                        // If assignment exists, update it to expired
                        DO.Assignment updatedAssignment = assignment with { CompletionTime = AdminManager.Now, Status = DO.Enums.TreatmentStatus.Expired };
                        s_dal.Assignment.Update(updatedAssignment);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the current status of a call based on its completion time and assignment status.
        /// </summary>
        /// <param name="id">The ID of the call.</param>
        /// <returns>The status of the call.</returns>
        internal static BO.CallStatus GetStatusCall(int id)
        {
            DO.Call? doCall = s_dal.Call.Read(id)!;
            DO.Assignment? doAssignment = s_dal.Assignment.Read(a => a.CallId == id);
            DateTime now = AdminManager.Now;

            TimeSpan? timeDifference = doCall.MaxCompletionTime - now;
            bool isExpired = doCall.MaxCompletionTime <= now;
            bool isAtRisk = timeDifference.HasValue && timeDifference <= s_dal.Config.RiskRange;

            if (doAssignment?.Status == DO.Enums.TreatmentStatus.Expired)
                return BO.CallStatus.Expired;
            if (doAssignment?.Status == DO.Enums.TreatmentStatus.CompletedOnTime)
                return BO.CallStatus.Closed;

            if (doAssignment == null ||
                doAssignment.Status == DO.Enums.TreatmentStatus.CanceledByVolunteer ||
                doAssignment.Status == DO.Enums.TreatmentStatus.CanceledByManager)
            {
                if (isExpired)
                    return BO.CallStatus.Expired;
                return isAtRisk ? BO.CallStatus.OpenAtRisk : BO.CallStatus.Open;
            }

            if (isExpired)
                return BO.CallStatus.Expired;
            if (isAtRisk)
                return BO.CallStatus.InProgressAtRisk;


            return BO.CallStatus.InProgress;
        }

        /// <summary>
        /// Retrieves the assignment history for a specific call.
        /// </summary>
        /// <param name="id">The ID of the call.</param>
        /// <returns>A list of assignment history for the call.</returns>
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

        /// <summary>
        /// Calculates the distance between a volunteer's current location and the location of a specific call.
        /// </summary>
        /// <param name="volunteerId">The ID of the volunteer.</param>
        /// <param name="callLat">The latitude of the call location.</param>
        /// <param name="callLong">The longitude of the call location.</param>
        /// <returns>The distance between the volunteer and the call location.</returns>
        internal static double CalculateDistance(int volunteerId, double callLat, double callLong)
        {
            var call = s_dal.Call.Read(volunteerId);
            if (call == null)
                throw new BO.BlDoesNotExistException($"Tutor with ID {volunteerId} not found");

            return GetDistance(call.Latitude, call.Longitude, callLat, callLong); // Calculate the distance using Haversine formula
        }

        /// <summary>
        /// Calculates the distance between two geographical coordinates.
        /// </summary>
        /// <param name="lat1">Latitude of the first point.</param>
        /// <param name="lon1">Longitude of the first point.</param>
        /// <param name="lat2">Latitude of the second point.</param>
        /// <param name="lon2">Longitude of the second point.</param>
        /// <returns>The distance between the two points in kilometers.</returns>
        private static double GetDistance(double lat1, double lon1, double lat2, double lon2)
        {
            double earthRadiusKm = 6371; // Radius of the Earth in kilometers
            double dLat = DegreesToRadians(lat2 - lat1);
            double dLon = DegreesToRadians(lon2 - lon1);
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return earthRadiusKm * c; // Return the distance in kilometers
        }

        /// <summary>
        /// Converts degrees to radians.
        /// </summary>
        /// <param name="degrees">The angle in degrees.</param>
        /// <returns>The angle in radians.</returns>
        private static double DegreesToRadians(double degrees)
        {
            return degrees * (Math.PI / 180.0);// Conversion formula
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
           
            address = address?.Trim();

            if (string.IsNullOrWhiteSpace(address))
            {
                throw new BO.BlValidationException("יש להזין כתובת.");
            }

            string url = $"https://nominatim.openstreetmap.org/search?q={Uri.EscapeDataString(address)}&format=json";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("VolunteerSystem/1.0");

                    var responseTask = client.GetAsync(url);
                    responseTask.Wait();

                    var response = responseTask.Result;

                    if (!response.IsSuccessStatusCode)
                        throw new BO.BlValidationException("קריאת המיקום נכשלה מהשרת.");

                    var jsonTask = response.Content.ReadAsStringAsync();
                    jsonTask.Wait();

                    var json = jsonTask.Result;

                    var result = JsonSerializer.Deserialize<NominatimResponse[]>(json);

                    if (result == null || result.Length == 0)
                        throw new BO.BlValidationException($"כתובת לא נמצאה: {address}");

                    double latitude = double.Parse(result[0].Lat, CultureInfo.InvariantCulture);
                    double longitude = double.Parse(result[0].Lon, CultureInfo.InvariantCulture);

                    return (latitude, longitude);
                }
            }
            catch (Exception ex)
            {
                throw new BO.BlValidationException($"שגיאה בעת קבלת קואורדינטות: {ex.Message}");
            }
        }



        /// <summary>
        /// Represents the structure of a geocoding response.
        /// </summary>
        private class NominatimResponse
        {
            [JsonPropertyName("lat")]
            public string Lat { get; set; }

            [JsonPropertyName("lon")]
            public string Lon { get; set; }

            [JsonPropertyName("display_name")]
            public string DisplayName { get; set; }
        }


        #endregion

        /// <summary>
        /// Validates if the input string is a valid number.
        /// </summary>
        /// <param name="input">The input string to validate.</param>
        /// <returns>True if the input is a valid number, false otherwise.</returns>
        public static bool IsValidNumber(string input)
        {
            var numberRegex = new Regex(@"^\d+$");
            return numberRegex.IsMatch(input); // Check if input is a number
        }

        /// <summary>
        /// Validates the properties of a call object.
        /// </summary>
        /// <param name="boCall">The call object to validate.</param>
        /// <param name="isUpdate">Flag indicating if this is an update operation (default is false).</param>
        public static void ValidateCall(BO.Call boCall, bool isUpdate = false)
        {
            // Validation - TimeOpen and MaxTimeFinishCall
            if (boCall.MaxCompletionTime.HasValue && boCall.OpenTime >= boCall.MaxCompletionTime)
            {
                throw new BO.BlValidationException("MaxTimeFinishCall must be later than TimeOpen.");
            }

            // Validation - FullAddressOfCall
            // Validation - FullAddressOfCall
            if (string.IsNullOrWhiteSpace(boCall.FullAddress))
            {
                throw new BO.BlValidationException("Full address of the call is required.");
            }
            else
            {
                try
                {
                    var (latitude, longitude) = GetCoordinates(boCall.FullAddress);
                    boCall.Latitude = latitude;
                    boCall.Longitude = longitude;
                }
                //catch (BO.BlValidationException)
                //{
                //    throw new BO.BlValidationException($"Invalid address: {boCall.FullAddress}");
                //}
                catch (Exception ex)
                {
                    throw new BO.BlValidationException($"שגיאה בעת בדיקת הכתובת: {ex.Message}");
                }

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

        /// <summary>
        /// Updates a DO.Call object if needed based on a BO.Call object.
        /// </summary>
        /// <param name="doCall">The existing DO.Call object.</param>
        /// <param name="boCAll">The BO.Call object containing new values.</param>
        /// <returns>The updated DO.Call object.</returns>
        public static DO.Call updateIfNeededDoCall(DO.Call doCall, BO.Call boCAll)
        {
            var copyDoCall = doCall;

            // Update properties if there are changes
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
            return copyDoCall; // Return updated DO.Call object
        }


        public static double CalculateHaversineDistance(double? lat1, double? lon1, double lat2, double lon2)
        {
            // Ensure that the first set of coordinates is not null
            if (lat1 == null || lon1 == null)
            {
                throw new BO.BlValidationException("Latitude and longitude of volunteer address cannot be null.");
            }

            const double EarthRadiusKm = 6371.0; // Radius of the Earth in kilometers

            // Convert latitude and longitude differences from degrees to radians
            double dLat = DegreesToRadians(lat2 - lat1.Value);
            double dLon = DegreesToRadians(lon2 - lon1.Value);

            // Apply the Haversine formula
            double a = Math.Pow(Math.Sin(dLat / 2), 2) +
                       Math.Cos(DegreesToRadians(lat1.Value)) * Math.Cos(DegreesToRadians(lat2)) *
                       Math.Pow(Math.Sin(dLon / 2), 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return EarthRadiusKm * c; // Return the calculated distance in kilometers
        }

        public static bool IsValidDateTime(DateTime? input)
        {
            if (!input.HasValue) return false;
            string formattedDate = input.Value.ToString("yyyy-MM-dd HH:mm:ss");
            return DateTime.TryParseExact(formattedDate, "yyyy-MM-dd HH:mm:ss",
                                          CultureInfo.InvariantCulture,
                                          DateTimeStyles.None,
                                          out _);
        }


    }
}
