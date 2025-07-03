using Helpers;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PL
{
    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => (value == null) ? Visibility.Collapsed : Visibility.Visible;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class SimulatorTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => (value is bool b && b) ? "עצור סימולטור" : "הפעל סימולטור";

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class SingleToCollectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Array.Empty<object>();
            return new[] { value };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => !(value is bool b) ? Binding.DoNothing : !b;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => !(value is bool b) ? Binding.DoNothing : !b;
    }
}

namespace PL.Call
{
    /// <summary>
    /// ממיר מזהה (Id) לכפתור: אם Id == 0 מחזיר "הוסף", אחרת מחזיר "עדכן".
    /// </summary>
    public class IdToButtonTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int id && id == 0)
                return "הוסף";
            return "עדכן";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class MinutesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime maxTime)
            {
                DateTime openTime = AdminManager.Now;
                if (parameter is DateTime providedOpenTime)
                    openTime = providedOpenTime;

                return ((int)(maxTime - openTime).TotalMinutes).ToString();
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (int.TryParse(value?.ToString(), out int minutes))
            {
                DateTime openTime = AdminManager.Now;
                if (parameter is DateTime providedOpenTime)
                    openTime = providedOpenTime;

                return openTime.AddMinutes(minutes);
            }
            return null;
        }
    }
}
