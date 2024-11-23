

using static DO.Enums;

namespace DO
{
    /// <summary>
    /// ישות הקצאת קריאה למתנדב, המייצגת את הקישור בין קריאה למתנדב שבחר לטפל בה.
    /// </summary>
    /// <param name="Id">מספר מזהה ייחודי של ההקצאה.</param>
    /// <param name="CallId">מספר מזהה של הקריאה שהמתנדב בחר לטפל בה.</param>
    /// <param name="VolunteerId">ת.ז של המתנדב.</param>
    /// <param name="EntryTime">זמן הכניסה לטיפול.</param>
    /// <param name="CompletionTime">זמן סיום הטיפול בפועל של הקריאה.</param>
    /// <param name="Status">סוג סיום הטיפול של הקריאה.</param>
    public record Assignment
    (
        int Id,
        int CallId,
        int VolunteerId,
        DateTime EntryTime,
        DateTime? CompletionTime = null,
        TreatmentStatus? Status = null
    )
    {

        /// קונסטרוקטור ברירת מחדל-ללא פרמטרים
        public Assignment() : this(0, 0, 0, DateTime.MinValue) { }
    }
}
