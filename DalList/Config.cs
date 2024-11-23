namespace Dal
{
    public static class Config
    {
        // הגדרה של מספרים רצים עבור ישויות שונות
        internal const int startCallId = 1000; // ערך התחלתי למזהה קריאה
        private static int nextCallId = startCallId; // ערך רץ

        // מאפיין שימשוך את המספר הרץ ויקדמו אוטומטית ב-1
        public static int NextCallId { get => nextCallId++; }

        internal const int startAssignmentId = 2000; // ערך התחלתי למזהה הקצאה
        private static int nextAssignmentId = startAssignmentId; // ערך רץ

        // מאפיין שימשוך את המספר הרץ ויקדמו אוטומטית ב-1
        public static int NextAssignmentId { get => nextAssignmentId++; }

        // שעון מערכת
        public static DateTime Clock { get; set; } = DateTime.Now;

        // טווח זמן סיכון
        internal static TimeSpan RiskRange { get; set; } = TimeSpan.FromMinutes(10);

        // מתודה לאיפוס משתני התצורה לערכים ההתחלתיים
        internal static void Reset()
        {
            nextCallId = startCallId;
            nextAssignmentId = startAssignmentId;
            Clock = DateTime.Now;
            RiskRange = TimeSpan.FromMinutes(10);
        }
    }
}
