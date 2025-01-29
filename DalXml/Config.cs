using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dal;

internal static class Config
{
    internal const string s_data_config = "data-config.xml"; // Config file for data
    internal const string s_volunteers_xml = "volunteers.xml"; // Volunteers data file
    internal const string s_calls_xml = "calls.xml"; // Calls data file
    internal const string s_assignments_xml = "assignments.xml"; // Assignments data file
    internal const int startCallId = 1000; // Starting value for call ID
    internal const int startAssignmentId = 2000; // Starting value for assignment ID

    public static int NextCallId
    {
        get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config, "NextCallId"); // Gets and increases the next call ID
        set => XMLTools.SetConfigIntVal(s_data_config, "NextCallId", value); // Sets the next call ID value
    }

    // Property for running assignment ID
    public static int NextAssignmentId
    {
        get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config, "NextAssignmentId"); // Gets and increases the next assignment ID
        set => XMLTools.SetConfigIntVal(s_data_config, "NextAssignmentId", value); // Sets the next assignment ID value
    }

    // Property for system clock time
    public static DateTime Clock
    {
        get => XMLTools.GetConfigDateVal(s_data_config, "Clock"); // Gets the current system time
        set => XMLTools.SetConfigDateVal(s_data_config, "Clock", value); // Sets the system clock time
    }

    // Property for risk time range
    public static TimeSpan RiskRange
    {
        get
        {
            int minutes = XMLTools.GetConfigIntVal(s_data_config, "RiskRangeMinutes"); // Gets risk range in minutes
            return TimeSpan.FromMinutes(minutes); // Converts to TimeSpan
        }
        set
        {
            int minutes = (int)value.TotalMinutes; // Converts TimeSpan to minutes
            XMLTools.SetConfigIntVal(s_data_config, "RiskRangeMinutes", minutes); // Sets risk range in minutes
        }
    }

    // Function to reset configuration variables to initial values
    internal static void Reset()
    {
        NextCallId = 1000; // Reset to initial call ID
        NextAssignmentId = 2000; // Reset to initial assignment ID
        Clock = DateTime.Now; // Reset clock to current time
        RiskRange = TimeSpan.Zero; // Reset risk range to zero
    }

}
