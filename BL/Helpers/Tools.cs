using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Linq.Expressions;
using System.Reflection;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Helpers
{
   

    public static class Tools
    {
        public class Location
        {
            public double Latitude { get; set; }
            public double Longitude { get; set; }
        }
        /// <summary>
        /// Gets the geographic coordinates (latitude, longitude) of a given address using OpenStreetMap API.
        /// </summary>
        public static async Task<Location?> GetLocationOfAddressAsync(string address)
        {
            try
            {
                using var client = new HttpClient();
                string url = $"https://nominatim.openstreetmap.org/search?q={Uri.EscapeDataString(address)}&format=json&limit=1";

                var response = await client.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                    return null;

                var content = await response.Content.ReadAsStringAsync();
                var results = JsonSerializer.Deserialize<List<NominatimResult>>(content);

                if (results is not null && results.Count > 0)
                {
                    return new Location
                    {
                        Latitude = double.Parse(results[0].lat),
                        Longitude = double.Parse(results[0].lon)
                    };
                }
            }
            catch
            {
                // Handle network error or invalid address gracefully
            }

            return null;
        }

        private class NominatimResult
        {
            public string lat { get; set; }
            public string lon { get; set; }
        }

        public static string ToStringProperty<T>(this T t)
        {
            string str = "";
            foreach (PropertyInfo item in t.GetType().GetProperties())
                str += "\n" + item.Name + ": " + item.GetValue(t, null);
            return str;
        }

        public static List<T> SortByEnum<T, TEnum>(List<T> list, TEnum? sortByField, bool descending = true)
            where TEnum : struct, Enum
        {
            if (list == null || list.Count == 0) return list;

            string propertyName = sortByField.ToString()!;
            PropertyInfo? prop = typeof(T).GetProperty(propertyName);
            if (prop == null) return list;

            var parameter = Expression.Parameter(typeof(T), "x");
            var propertyAccess = Expression.Property(parameter, prop);
            var conversion = Expression.Convert(propertyAccess, typeof(object));
            var lambda = Expression.Lambda<Func<T, object>>(conversion, parameter).Compile();

            return descending
                ? list.OrderByDescending(lambda).ToList()
                : list.OrderBy(lambda).ToList();
        }

        public static IEnumerable<T> FilterList<T, TEnum>(IEnumerable<T> list, TEnum? filterByField, object? value)
            where TEnum : struct, Enum
        {
            string propertyName = filterByField.ToString()!;
            PropertyInfo? prop = typeof(T).GetProperty(propertyName);
            if (prop != null)
            {
                return list.Where(item =>
                {
                    var propValue = prop.GetValue(item);

                    if (propValue != null && propValue.GetType() != value.GetType())
                    {
                        try
                        {
                            propValue = Convert.ChangeType(propValue, value.GetType());
                        }
                        catch
                        {
                            return false;
                        }
                    }

                    return propValue != null && propValue.Equals(value);
                }).ToList();
            }
            return list;
        }
    }
}
