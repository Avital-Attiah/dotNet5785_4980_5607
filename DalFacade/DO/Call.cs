using static DO.Enums;

namespace DO
{
    /// <summary>
    /// ישות קריאה המכילה את פרטי הקריאה, כולל מזהה רץ ייחודי, סוג, כתובת, זמן פתיחה ועוד.
    /// </summary>
    /// <param name="Id">מזהה ייחודי לקריאה.</param>
    /// <param name="CallType">סוג הקריאה (לפי סוג המערכת).</param>
    /// <param name="FullAddress">כתובת מלאה של הקריאה.</param>
    /// <param name="OpenTime">זמן פתיחת הקריאה.</param>
    /// <param name="Description">תיאור מילולי של הקריאה (ברירת מחדל: null).</param>
    /// <param name="Latitude">קו רוחב של מיקום הקריאה (ברירת מחדל: 0.0).</param>
    /// <param name="Longitude">קו אורך של מיקום הקריאה (ברירת מחדל: 0.0).</param>
    /// <param name="MaxCompletionTime">זמן מקסימלי לסיום הקריאה (אם קיים, ברירת מחדל: null).</param>
    /// <param name="isEmergency">תכונה לבדיקה אם הקריאה דחופה או לא</param>
    public record Call
    (
        int Id,
        CallType CallType,
        string FullAddress,
        DateTime OpenTime,
        bool isEmergency=false,
        string? Description = null,
        double Latitude = 0.0,
        double Longitude = 0.0,
        DateTime? MaxCompletionTime = null
    )
    {
        /// קונסטרוקטור ברירת מחדל (ללא פרמטרים).
        public Call() : this(0, default, "", DateTime.MinValue) { }

   
    }
}
