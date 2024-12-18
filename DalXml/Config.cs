using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dal;

internal static class Config
{
    internal const string s_data_config = "data-config";
    internal const string s_volunteers_xml = "volunteers.xml";
    internal const string s_calls_xml = "calls.xml";
    internal const string s_assignments_xml = "assignments.xml";
    internal const int startCallId = 1000; // ערך התחלתי למזהה קריאה
    internal const int startAssignmentId = 2000; // ערך התחלתי למזהה הקצאה
    

    public static int NextCallId
    {
        get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config, "NextCallId");
        set => XMLTools.SetConfigIntVal(s_data_config, "NextCallId", value);
    }

    // תכונה למספר מזהה רץ להקצאה
    public static int NextAssignmentId
    {
        get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config, "NextAssignmentId");
        set => XMLTools.SetConfigIntVal(s_data_config, "NextAssignmentId", value);
    }

    // תכונה לזמן שעון המערכת
    public static DateTime Clock
    {
        get => XMLTools.GetConfigDateVal(s_data_config, "Clock");
        set => XMLTools.SetConfigDateVal(s_data_config, "Clock", value);
    }

    // תכונה לטווח זמן סיכון
    public static TimeSpan RiskRange
    {
        get
        {
            int minutes = XMLTools.GetConfigIntVal(s_data_config, "RiskRangeMinutes");
            return TimeSpan.FromMinutes(minutes);
        }
        set
        {
            int minutes = (int)value.TotalMinutes;
            XMLTools.SetConfigIntVal(s_data_config, "RiskRangeMinutes", minutes);
        }
    }

    // פונקציה לאיפוס משתני התצורה לערכים התחלתיים
    internal static void Reset()
    {
        XMLTools.SetConfigIntVal(s_data_config, "NextCallId", 1000); // ערך התחלתי למזהה קריאה
        XMLTools.SetConfigIntVal(s_data_config, "NextAssignmentId", 2000); // ערך התחלתי למזהה הקצאה
        XMLTools.SetConfigDateVal(s_data_config, "Clock", DateTime.Now); // זמן נוכחי
        XMLTools.SetConfigIntVal(s_data_config, "RiskRangeMinutes", 10); // טווח זמן בסיכון
    }
}










