
namespace DalApi;
using DO;
public interface IVolunteer
{
    void Create(Volunteer item); // יוצר אובייקט ישות חדש ב-DAL
    Volunteer? Read(int id); // קורא אובייקט ישות לפי מזהה שלו
    List<Volunteer> ReadAll(); // שלב 1 בלבד, קורא את כל אובייקטי הישות
    void Update(Volunteer item); // מעדכן אובייקט ישות
    void Delete(int id); // מוחק אובייקט לפי המזהה שלו
    void DeleteAll(); // מוחק את כל אובייקטי הישות
    void Print(Volunteer item); // מדפיס את הפריט
    //public void SetInitialPassword(int id, string password);/// יצירת סיסמה ראשונית למתנדב
    //public void UpdatePassword(int id, string newPassword);  /// עדכון סיסמת המתנדב
    //public bool IsPasswordStrong(string password);// בדיקה אם הסיסמה חזקה מספיק
}
