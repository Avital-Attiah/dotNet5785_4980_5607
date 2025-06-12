using Helpers;
using System;
using System.Collections.Generic;

namespace BO
{
    public class Call
    {
        public int Id { get; init; }

        public CallType CallType { get; set; } // סוג הקריאה
        public string FullAddress { get; set; } // כתובת מלאה
        public string? Address { get; set; } // ✅ כתובת נוספת לצורך תצוגה
        public string? Description { get; set; } // תיאור מילולי
        public string? Type { get; set; } // ✅ סוג נוסף לצורך binding ב־XAML

        public DateTime OpenTime { get; set; } // מתי נפתחה הקריאה
        public DateTime? MaxCompletionTime { get; set; } // זמן מקסימלי לסיום
        public DateTime? MaxTimeToFinish { get; set; } // ✅ תאום לשם אחר שמופיע ב־XAML

        public CallStatus Status { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public List<CallAssignInList>? ListAssignments { get; set; }

        public override string ToString() => Tools.ToStringProperty(this);
    }
}
