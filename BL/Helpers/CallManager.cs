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
using System.Net.Http; // שים לב: צריך גם using ל־HttpClient

namespace Helpers
{
    // Static class responsible for managing calls, assignments, and validations
    public static class CallManager
    {
        internal static ObserverManager Observers = new(); //stage 5 
        private static IDal s_dal = Factory.Get; // Data access layer

        /// <summary>
        /// מחזירה את הקריאות הפתוחות שמתאימות למתנדב, ושיש להן קואורדינטות מחושבות.
        /// </summary>
        internal static IEnumerable<BO.CallInList> GetOpenCallsForVolunteer(int volunteerId)
        {
            DO.Volunteer? volunteer;
            lock (AdminManager.BlMutex)
                volunteer = s_dal.Volunteer.Read(volunteerId);

            if (volunteer == null || !volunteer.Latitude.HasValue || !volunteer.Longitude.HasValue)
                return Enumerable.Empty<BO.CallInList>();
            UpdateExpiredOpenCalls();
            List<DO.Call> openCalls;
            lock (AdminManager.BlMutex)
            {
                openCalls = s_dal.Call
                    .ReadAll(c => c.MaxCompletionTime > AdminManager.Now)
                    .Where(c => c.Latitude != null && c.Longitude != null)
                    .ToList();
            }

            List<BO.CallInList> result = new();

            foreach (var call in openCalls)
            {
                double distance = CalculateHaversineDistance(volunteer.Latitude!.Value,volunteer.Longitude!.Value,call.Latitude!.Value,call.Longitude!.Value);


                if (distance <= volunteer.MaxDistance)
                {
                    int assignmentsCount;
                    string lastVolunteerName = null;
                    TimeSpan? timeToComplete = null;

                    lock (AdminManager.BlMutex)
                    {
                        var assignments = s_dal.Assignment.ReadAll(a => a.CallId == call.Id).ToList();
                        assignmentsCount = assignments.Count;

                        var last = assignments.OrderByDescending(a => a.EntryTime).FirstOrDefault();
                        if (last != null && last.VolunteerId != 0)
                        {
                            var vol = s_dal.Volunteer.Read(last.VolunteerId);
                            lastVolunteerName = vol?.FullName;

                            if (last.CompletionTime != null)
                                timeToComplete = last.CompletionTime - call.OpenTime;
                        }
                    }

                    result.Add(new BO.CallInList
                    {
                        Id = null,
                        CallId = call.Id,
                        CallType = (BO.CallType)call.CallType,
                        OpenTime = call.OpenTime,
                        TimeRemaining = call.MaxCompletionTime - AdminManager.Now,
                        LastAssignedVolunteerName = lastVolunteerName,
                        TimeToComplete = timeToComplete,
                        Status = GetStatusCall(call.Id),
                        AssignmentsCount = assignmentsCount,
                        CoordinateStatusMessage = (call.Latitude == null || call.Longitude == null)? "קואורדינטות לא זמינות (השאילתא לא הושלמה או נכשלה)": null
                    });
                }
            }

            return result;
        }

        /// <summary>
        /// Updates the status of open calls that have expired by comparing their completion time to the current time.
        /// </summary>
        public static bool UpdateExpiredOpenCalls()
        {
            bool changed = false;

            List<DO.Call> list;
            lock (AdminManager.BlMutex)
            {
                list = s_dal.Call.ReadAll(c => c.MaxCompletionTime <= AdminManager.Now).ToList();
            }

            foreach (var doCall in list)
            {
                DO.Assignment? assignment;
                lock (AdminManager.BlMutex)
                {
                    assignment = s_dal.Assignment.Read(a => a.CallId == doCall.Id);
                }

                if (assignment == null)
                {
                    DO.Assignment doAssignment = new(
                        0,
                        doCall.Id,
                        0,
                        AdminManager.Now,
                        AdminManager.Now,
                        DO.Enums.TreatmentStatus.Expired);

                    lock (AdminManager.BlMutex)
                        s_dal.Assignment.Create(doAssignment);

                    Console.WriteLine($"❌ קריאה {doCall.Id} פגה תוקפה – נוצרה השמה Expired");
                    changed = true;
                }
                else if (assignment.CompletionTime == null)
                {
                    DO.Assignment updatedAssignment = assignment with
                    {
                        CompletionTime = AdminManager.Now,
                        Status = DO.Enums.TreatmentStatus.Expired
                    };

                    lock (AdminManager.BlMutex)
                        s_dal.Assignment.Update(updatedAssignment);

                    Console.WriteLine($"❌ קריאה {doCall.Id} פגה תוקפה – ההשמה עודכנה ל־Expired");
                    changed = true;
                }
            }

            return changed;
        }





        /// <summary>
        /// Gets the current status of a call based on its completion time and assignment status.
        /// </summary>
        public static BO.CallStatus GetStatusCall(int id)
        {
            DO.Call? doCall;
            DO.Assignment? doAssignment;
            TimeSpan? timeDifference;
            bool isExpired;
            bool isAtRisk;

            lock (AdminManager.BlMutex) //stage 7
            {
                doCall = s_dal.Call.Read(id);
                if (doCall == null)
                    throw new BO.BlDoesNotExistException($"Call with ID={id} does not exist");

                doAssignment = s_dal.Assignment.Read(a => a.CallId == id);
                DateTime now = AdminManager.Now;
                timeDifference = doCall.MaxCompletionTime - now;
                isExpired = doCall.MaxCompletionTime <= now;
                isAtRisk = timeDifference.HasValue && timeDifference <= s_dal.Config.RiskRange;
            }

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
        public static List<BO.CallAssignInList> GetAssignmentsHistory(int id)
        {
            lock (AdminManager.BlMutex) //stage 7
            {
                return s_dal.Assignment.ReadAll(a => a.CallId == id)
                    .Select(item =>
                    {
                        var volunteer = s_dal.Volunteer.Read(item.VolunteerId);
                        return new BO.CallAssignInList
                        {
                            VolunteerId = item.VolunteerId,
                            VolunteerName = volunteer?.FullName ?? "(מתנדב לא קיים)",
                            StartTime = item.EntryTime,
                            EndTime = item.CompletionTime,
                            CompletionType = (BO.CompletionType?)item.Status
                        };
                    })
                    .ToList();
            }
        }


        /// <summary>
        /// Calculates the distance between a volunteer's current location and the location of a specific call.
        /// </summary>
        public static double CalculateDistance(int volunteerId, double callLat, double callLong)
        {
            DO.Volunteer? volunteer;
            lock (AdminManager.BlMutex) //stage 7
            {
                volunteer = s_dal.Volunteer.Read(volunteerId);
            }

            if (volunteer == null)
                throw new BO.BlDoesNotExistException($"Volunteer with ID {volunteerId} not found");

            if (!volunteer.Latitude.HasValue || !volunteer.Longitude.HasValue)
                throw new Exception("Volunteer location is not set");

            return GetDistance(volunteer.Latitude.Value, volunteer.Longitude.Value, callLat, callLong);
        }

        /// <summary>
        /// Calculates the distance between two geographical coordinates.
        /// </summary>
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
        private static double DegreesToRadians(double degrees)
        {
            return degrees * (Math.PI / 180.0);// Conversion formula
        }

        #region check address

        /// <summary>
        /// Retrieves the geographical coordinates (latitude and longitude) for a given address.
        /// </summary>
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

        public static bool IsValidNumber(string input)
        {
            var numberRegex = new Regex(@"^\d+$");
            return numberRegex.IsMatch(input); // Check if input is a number
        }

        public static void ValidateCall(BO.Call boCall, bool isUpdate = false)
        {
            AdminManager.ThrowOnSimulatorIsRunning();

            if (boCall.MaxCompletionTime.HasValue && boCall.OpenTime >= boCall.MaxCompletionTime)
            {
                throw new BO.BlValidationException("MaxTimeFinishCall must be later than TimeOpen.");
            }

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
                catch (Exception ex)
                {
                    throw new BO.BlValidationException($"שגיאה בעת בדיקת הכתובת: {ex.Message}");
                }
            }

            if (string.IsNullOrEmpty(boCall.Description))
            {
                throw new BO.BlValidationException("Verbal description is required.");
            }

            if (!VolunteerManager.IsValidEnum<BO.CallType>((int)boCall.CallType))
            {
                throw new BO.BlValidationException($"Invalid TypeCall value: {boCall.CallType}");
            }
        }

        public static DO.Call updateIfNeededDoCall(DO.Call doCall, BO.Call boCAll)
        {
            AdminManager.ThrowOnSimulatorIsRunning();

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
            return copyDoCall; // Return updated DO.Call object
        }

        public static double CalculateHaversineDistance(double? lat1, double? lon1, double lat2, double lon2)
        {
            if (lat1 == null || lon1 == null)
            {
                throw new BO.BlValidationException("Latitude and longitude of volunteer address cannot be null.");
            }

            const double EarthRadiusKm = 6371.0; // Radius of the Earth in kilometers

            double dLat = DegreesToRadians(lat2 - lat1.Value);
            double dLon = DegreesToRadians(lon2 - lon1.Value);

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
        internal static async Task UpdateCoordinatesForCallAddressAsync(DO.Call doCall, IDal dal)
        {
            try
            {
                // קבלת קואורדינטות מהכתובת (סינכרוני)
                var (lat, lon) = GetCoordinates(doCall.FullAddress); // ←←← זה התיקון

                // יצירת עותק מעודכן של הקריאה עם הקואורדינטות
                var updatedCall = doCall with
                {
                    Latitude = lat,
                    Longitude = lon
                };

                // עדכון הקריאה במסד הנתונים
                lock (AdminManager.BlMutex)
                    dal.Call.Update(updatedCall);

                // יידוע כל התצוגות שיש עדכון לפריט הספציפי וגם לרשימה
                Observers.NotifyItemUpdated(doCall.Id);
                Observers.NotifyListUpdated();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to get coordinates for Call ID {doCall.Id}: {ex.Message}");

                // ליידע שהתצוגה צריכה להתעדכן (כדי שיראו ToolTip למשל)
                Observers.NotifyItemUpdated(doCall.Id);
                Observers.NotifyListUpdated();
            }
        }

        internal static async Task<Tools.Location?> GetLocationOfAddressAsync(string address)
        {
            try
            {
                return await Tools.GetLocationOfAddressAsync(address); // קריאה אסינכרונית לכלי שמחשב קואורדינטות
            }
            catch
            {
                return null; // במקרה של כשל
            }
        }
        internal static async Task UpdateCoordinatesForCallAsync(DO.Call doCall)
        {
            if (string.IsNullOrWhiteSpace(doCall.FullAddress)) return;

            var location = await Tools.GetLocationOfAddressAsync(doCall.FullAddress);
            if (location != null)
            {
                doCall = doCall with { Latitude = location.Latitude, Longitude = location.Longitude };
                lock (AdminManager.BlMutex)
                    s_dal.Call.Update(doCall);

                Observers.NotifyItemUpdated(doCall.Id);
                Observers.NotifyListUpdated();
            }
        }
        internal static async Task<Tools.Location?> GetCoordinatesAsync(string address)
        {
            try
            {
                return await Tools.GetLocationOfAddressAsync(address);
            }
            catch
            {
                return null;
            }
        }



    }



}
