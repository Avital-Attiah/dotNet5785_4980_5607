
using static DO.Enums;

namespace DO
{
    /// <summary>
    /// ישות המתנדב, מכילה את פרטי המתנדב כולל ת.ז ייחודית ונתונים נוספים על הפעילות של המתנדב.
    /// </summary>
    /// <param name="Id">ת.ז מתנדב, מזהה ייחודי למתנדב.</param>
    /// <param name="FullName">שם מלא של המתנדב (שם פרטי ומשפחה).</param>
    /// <param name="Phone">טלפון סלולרי תקני של המתנדב.</param>
    /// <param name="Email">כתובת דואר אלקטרוני של המתנדב.</param>
    /// <param name="Password">סיסמת המתנדב, יכולה להיות null בהתחלה.</param>
    /// <param name="CurrentAddress">כתובת מלאה של המתנדב.</param>
    /// <param name="Latitude">קו רוחב של המתנדב, יחושב על פי הכתובת.</param>
    /// <param name="Longitude">קו אורך של המתנדב, יחושב על פי הכתובת.</param>
    /// <param name="Role">תפקיד המתנדב (מנהל או מתנדב).</param>
    /// <param name="IsActive">האם המתנדב פעיל בארגון.</param>
    /// <param name="MaxDistance">המרחק המרבי שבו המתנדב יכול לקבל קריאות.</param>
    /// <param name="DistanceType">סוג המרחק (אוויר, הליכה, נסיעה).</param>
    public record Volunteer
    (
        int Id ,
        string FullName,
        string Phone,
        string Email,
        string? Password = null,
        string? CurrentAddress = null,
        double? Latitude = null,
        double? Longitude = null,
        Role Role= default,
        bool IsActive = true,
        double? MaxDistance = null,
        DistanceType DistanceType = default
    )
    {
        // בנאי ריק (ברירת מחדל) - נדרש לכל ישות מסוג record.
        public Volunteer() : this(0, "", "", "" ) { }
    }
   
}
