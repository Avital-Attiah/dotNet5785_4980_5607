namespace DalTest;
using Dal;
using DalApi;
using DO;
using static DO.Enums;



public static class Initialization
{
    //private static IAssignment? s_dalAssignment; // ממשק למשימות
    //private static ICall? s_dalCall;             // ממשק לקריאות
    //private static IConfig? s_dalConfig;         // ממשק תצורה
    //private static IVolunteer? s_dalVolunteer;   // ממשק למתנדבים
    private static IDal? s_dal;

    private static readonly Random s_rand = new(); // מחולל מספרים רנדומליים
    public static void Do(IDal dal) 
    {


        // הצבת הפרמטרים בשדות הסטטיים של המחלקה
        //s_dalAssignment = dalAssignment ?? throw new NullReferenceException("Assignment DAL object cannot be null!");
        //s_dalCall = dalCall ?? throw new NullReferenceException("Call DAL object cannot be null!");
        //s_dalConfig = dalConfig ?? throw new NullReferenceException("Config DAL object cannot be null!");
        //s_dalVolunteer = dalVolunteer ?? throw new NullReferenceException("Volunteer DAL object cannot be null!");
        s_dal = dal ?? throw new NullReferenceException("DAL object can not be null!"); // stage 2
        // איפוס נתוני תצורה ורשימות
        Console.WriteLine("Resetting configuration and clearing all data...");
        //s_dalConfig.Reset(); // איפוס הגדרות התצורה
        //s_dalVolunteer.DeleteAll(); // מחיקת כל המתנדבים
        //s_dalCall.DeleteAll(); // מחיקת כל הקריאות
        //s_dalAssignment.DeleteAll(); // מחיקת כל ההקצאות
        s_dal.ResetDB();//stage 2

        // אתחול הנתונים ברשימות
        Console.WriteLine("Initializing Volunteers list...");
        CreateVolunteer(); // יצירת מתנדבים

        Console.WriteLine("Initializing Calls list...");
        createCall(); // יצירת קריאות

        Console.WriteLine("Initializing Assignments list...");
        CreateAssignment(); // יצירת הקצאות

        Console.WriteLine("Initialization completed successfully!");
    }

    //יוצר מתנדבים ע''פ הדרישות
    private static void CreateVolunteer()
    {
        string[] names = { "John Doe", "Jane Smith", "Michael Brown", "Emily White", "Daniel Johnson",
                       "Sarah Davis", "David Wilson", "Jessica Martinez", "James Anderson", "Laura Thomas",
                       "Chris Taylor", "Sophia Lee", "Brian Harris", "Olivia Moore", "Matthew Clark" };

        string[] emails = { "john@example.com", "jane@example.com", "michael@example.com", "emily@example.com", "daniel@example.com",
                        "sarah@example.com", "david@example.com", "jessica@example.com", "james@example.com", "laura@example.com",
                        "chris@example.com", "sophia@example.com", "brian@example.com", "olivia@example.com", "matthew@example.com" };

        string[] phoneNumbers = { "0501234567", "0529876543", "0534567890", "0541239876", "0557894321",
                              "0561236547", "0576543210", "0584321987", "0591238765", "0524567891",
                              "0537891234", "0549876123", "0553219876", "0506785432", "0512345678" };

        var addresses = new[] {
        new { Address = "Tel Aviv", Latitude = 32.0853, Longitude = 34.7818 },
        new { Address = "Jerusalem", Latitude = 31.7683, Longitude = 35.2137 },
        new { Address = "Haifa", Latitude = 32.7940, Longitude = 34.9896 }
    };

        var random = new Random();
        Role ro = Role.manager;
        for (int i = 0; i < names.Length; i++)
        {

            int id;
            do
            {
                id = random.Next(200000000, 400000000);
            } while (s_dal.Volunteer != null && s_dal.Volunteer.Read(id) != null);  // קריאה למתנדב על ידי ה-DAL

            // יצירת אובייקט מתנדב חדש
            var volunteer = new Volunteer
            {
                Id = id,
                FullName = names[i],
                Email = emails[i],
                Phone = phoneNumbers[i],
                CurrentAddress = addresses[i % addresses.Length].Address,
                Latitude = addresses[i % addresses.Length].Latitude,
                Longitude = addresses[i % addresses.Length].Longitude,
                Role = ro,
                MaxDistance = random.Next(5, 50), // רדיוס אקראי בק"מ
                DistanceType = DistanceType.Air // מרחק אווירי
            };

            // קריאה ל-DAL כדי ליצור את המתנדב
            if (s_dal.Volunteer != null)
                s_dal!.Volunteer.Create(volunteer);
            ro = Role.Volunteer;
        }
    }

    //יוצר קריאות ע''פ הדרישות
    private static void createCall()
    {
        string[] descriptions = {
        "Emotional support", "Crisis intervention", "Suicide prevention", "Loneliness support",
        "Guidance for families", "Urgent assistance", "Mental health advice", "Stress management" };
        var addresses = new[] {
        new { Address = "Tel Aviv", Latitude = 32.0853, Longitude = 34.7818 },
        new { Address = "Jerusalem", Latitude = 31.7683, Longitude = 35.2137 },
        new { Address = "Haifa", Latitude = 32.7940, Longitude = 34.9896 }
    };
        // יצירת אובייקט random מחוץ ללולאה
        Random random = new Random();

        for (int i = 0; i < 50; i++)
        {
            var startTime = s_dal!.Config.Clock.AddMinutes(-random.Next(1, 1440)); // פתיחה עד 24 שעות אחורה
            var endTime = random.Next(0, 2) == 0
                ? startTime.AddMinutes(random.Next(10, 120)) // זמן סיום רנדומלי
                : (DateTime?)null;

            // יצירת אובייקט Call עם כל המאפיינים הדרושים
            var call = new Call
            {
                Id = s_dal!.Config.NextCallId,  // משתמשים ב-NextCallId ליצירת מזהה ייחודי
                CallType = CallType.Emergency,  // או כל סוג קריאה שדרוש
                FullAddress = addresses[i % addresses.Length].Address,  // לדוגמה, צריך להניח כתובת רנדומלית או נכונה
                OpenTime = startTime,  // זמן פתיחה
                isEmergency = true,  // ערך לדוגמה, צריך להתאים לפי הצורך
                Description = descriptions[random.Next(descriptions.Length)],  // בחירת תיאור רנדומלי
                Latitude = addresses[i % addresses.Length].Latitude,
                Longitude = addresses[i % addresses.Length].Longitude,
                MaxCompletionTime = endTime  // זמן סיום אם יש
            };
            if (s_dal.Call != null)
                // יצירת קריאה עם ה-Call שנוצר
                s_dal!.Call.Create(call);  // קריאה לפונקציה המתאימה עם אובייקט call

        }

    }

    //יוצר הקצאות ע''פ הדרישות
    private static void CreateAssignment()
    {
        // יצירת אובייקט random מחוץ ללולאה
        Random random = new Random();

        // וידוא שיש קריאות ומתנדבים קיימים במערכת
        if (s_dal.Call == null || s_dal.Volunteer == null) return;

        // איסוף קריאות ומתנדבים מהרשימה
        var calls = s_dal!.Call.ReadAll();
        var volunteers = s_dal!.Volunteer.ReadAll();

        if (calls == null || volunteers == null || !calls.Any() || !volunteers.Any()) return;

        for (int i = 0; i < 50; i++)
        {
            // בחירת קריאה רנדומלית מהרשימה
            var call = calls.ElementAt(random.Next(calls.Count()));

            // בחירת מתנדב רנדומלי מהרשימה
            var volunteer = volunteers.ElementAt(random.Next(volunteers.Count()));

            // הגדרת זמן כניסה לטיפול
            var entryTime = call.OpenTime.AddMinutes(random.Next(1, (int)(call.MaxCompletionTime?.Subtract(call.OpenTime).TotalMinutes ?? 1440)));

            // הגדרת זמן סיום טיפול
            DateTime? completionTime = random.Next(0, 2) == 0
                ? entryTime.AddMinutes(random.Next(5, 120)) // זמן סיום רנדומלי
                : null;

            // קביעת סטטוס סיום הטיפול
            TreatmentStatus? status = null;

            if (completionTime.HasValue)
            {
                if (completionTime <= call.MaxCompletionTime)
                {
                    status = TreatmentStatus.CompletedOnTime; // קריאה שטופלה בזמן
                }
                else
                {
                    status = TreatmentStatus.Expired; // קריאה שפג תוקפה
                }
            }
            else
            {
                // סטטוס ביטול רנדומלי
                status = random.Next(0, 2) == 0
                    ? TreatmentStatus.CanceledByVolunteer // ביטול על ידי מתנדב
                    : TreatmentStatus.CanceledBymanager; // ביטול על ידי מנהל
            }

            // יצירת האובייקט Assignment
            var assignment = new Assignment(
                Id: s_dal!.Config.NextAssignmentId, // מזהה רץ
                CallId: call.Id, // מזהה קריאה
                VolunteerId: volunteer.Id, // ת.ז מתנדב
                EntryTime: entryTime, // זמן כניסה לטיפול
                CompletionTime: completionTime, // זמן סיום טיפול
                Status: status // סטטוס סיום
            );

            // הוספת ההקצאה לרשימה
            if (s_dal.Assignment != null)
                s_dal!.Assignment.Create(assignment); // יצירת ההקצאה ברשימה
        }
    }





}