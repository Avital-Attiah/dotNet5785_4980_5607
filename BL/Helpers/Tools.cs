
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Linq.Expressions;
using System.Reflection;
﻿using System.Linq.Expressions;
using System.Reflection;
namespace Helpers;

internal static class Tools
{
    public static string ToStringProperty<T>(this T t)
    {
        string str = "";
        foreach (PropertyInfo item in t.GetType().GetProperties())
            str += "\n" + item.Name +
            ": " + item.GetValue(t, null);
        return str;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TEnum"></typeparam>
    /// <param name="list"></param>
    /// <param name="sortByField"></param>
    /// <param name="descending"></param>
    /// <returns></returns>
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

