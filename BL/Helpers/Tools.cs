using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Linq.Expressions;
using System.Reflection;
using System.Linq.Expressions;
using System.Reflection;

namespace Helpers
{
    // Static helper class containing useful extension methods and utilities
    internal static class Tools
    {
        /// <summary>
        /// Extension method that converts all properties of an object to a formatted string.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="t">The object whose properties are to be converted to a string.</param>
        /// <returns>A string representation of the object's properties.</returns>
        public static string ToStringProperty<T>(this T t)
        {
            string str = "";
            // Loop through each property of the object and append it to the result string
            foreach (PropertyInfo item in t.GetType().GetProperties())
                str += "\n" + item.Name + ": " + item.GetValue(t, null); // Append property name and value
            return str;
        }

        /// <summary>
        /// Sorts a list of objects by a property corresponding to an enum value.
        /// </summary>
        /// <typeparam name="T">The type of the objects in the list.</typeparam>
        /// <typeparam name="TEnum">The enum type used to specify the property to sort by.</typeparam>
        /// <param name="list">The list of objects to be sorted.</param>
        /// <param name="sortByField">The enum value that specifies the property to sort by.</param>
        /// <param name="descending">A boolean value that specifies whether to sort in descending order (default is true).</param>
        /// <returns>A new sorted list of objects.</returns>
        public static List<T> SortByEnum<T, TEnum>(List<T> list, TEnum? sortByField, bool descending = true)
            where TEnum : struct, Enum
        {
            if (list == null || list.Count == 0) return list; // Return the list if it's null or empty

            string propertyName = sortByField.ToString()!;
            PropertyInfo? prop = typeof(T).GetProperty(propertyName); // Get the property based on the enum value
            if (prop == null) return list; // Return the list if the property doesn't exist

            // Create an expression to access the property dynamically
            var parameter = Expression.Parameter(typeof(T), "x");
            var propertyAccess = Expression.Property(parameter, prop);
            var conversion = Expression.Convert(propertyAccess, typeof(object));
            var lambda = Expression.Lambda<Func<T, object>>(conversion, parameter).Compile();

            // Sort the list based on the property, in ascending or descending order
            return descending
                ? list.OrderByDescending(lambda).ToList()
                : list.OrderBy(lambda).ToList();
        }

        /// <summary>
        /// Filters a list of objects based on an enum property value.
        /// </summary>
        /// <typeparam name="T">The type of the objects in the list.</typeparam>
        /// <typeparam name="TEnum">The enum type used to specify the property to filter by.</typeparam>
        /// <param name="list">The list of objects to be filtered.</param>
        /// <param name="filterByField">The enum value that specifies the property to filter by.</param>
        /// <param name="value">The value to filter the property by.</param>
        /// <returns>A new filtered list of objects.</returns>
        public static IEnumerable<T> FilterList<T, TEnum>(IEnumerable<T> list, TEnum? filterByField, object? value)
            where TEnum : struct, Enum
        {
            string propertyName = filterByField.ToString()!;
            PropertyInfo? prop = typeof(T).GetProperty(propertyName); // Get the property based on the enum value
            if (prop != null)
            {
                // Filter the list based on the value of the specified property
                return list.Where(item =>
                {
                    var propValue = prop.GetValue(item);

                    // Handle type mismatch and attempt to convert if necessary
                    if (propValue != null && propValue.GetType() != value.GetType())
                    {
                        try
                        {
                            propValue = Convert.ChangeType(propValue, value.GetType());
                        }
                        catch
                        {
                            return false; // Return false if conversion fails
                        }
                    }

                    return propValue != null && propValue.Equals(value); // Return true if property value matches the filter value
                }).ToList();
            }
            return list; // Return the list if the property doesn't exist
        }
    }
}
