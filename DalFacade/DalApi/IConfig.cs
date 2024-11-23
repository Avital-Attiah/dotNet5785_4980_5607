namespace DalApi;
using DO;

public interface IConfig
{
    
    // ערך התחלתי למזהה קריאה
    int StartCallId { get; }

    // מזהה רץ למזהה קריאה
    int NextCallId { get; }

    // ערך התחלתי למזהה הקצאה
    int StartAssignmentId { get; }

    // מזהה רץ למזהה הקצאה
    int NextAssignmentId { get; }

    // שעון מערכת
    DateTime Clock { get; set; }

    // טווח זמן סיכון
    TimeSpan RiskRange { get; set; }

    // מתודה לאיפוס משתני התצורה לערכים ההתחלתיים
    void Reset();
}
