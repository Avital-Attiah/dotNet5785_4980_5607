using System.Runtime.CompilerServices;

namespace Dal
{
    internal static class Config
    {
        // Starting ID for calls
        internal const int startCallId = 1000;

        // Running call ID counter
        private static int nextCallId = startCallId;

        // Gets and increments the next available call ID
        public static int NextCallId
        {
            [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
            get => nextCallId++;
        }

        // Starting ID for assignments
        internal const int startAssignmentId = 2000;

        // Running assignment ID counter
        private static int nextAssignmentId = startAssignmentId;

        // Gets and increments the next available assignment ID
        public static int NextAssignmentId
        {
            [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
            get => nextAssignmentId++;
        }

        // System clock (used for time-based operations)
        private static DateTime clock = DateTime.Now;
        public static DateTime Clock
        {
            [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
            get => clock;

            [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
            set => clock = value;
        }

        // Risk threshold range for call expiration logic
        private static TimeSpan riskRange = TimeSpan.FromMinutes(10);
        internal static TimeSpan RiskRange
        {
            [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
            get => riskRange;

            [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
            set => riskRange = value;
        }

        // Resets configuration values to their default state
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        internal static void Reset()
        {
            nextCallId = startCallId;
            nextAssignmentId = startAssignmentId;
            Clock = DateTime.Now;
            RiskRange = TimeSpan.FromMinutes(10);
        }
    }
}
