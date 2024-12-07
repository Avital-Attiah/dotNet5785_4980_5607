
namespace DalApi;
using DO;
public interface IVolunteer : ICrud<Volunteer>
{
    void Print(Volunteer item); // מדפיס את הפריט
    //public void SetInitialPassword(int id, string password);/// יצירת סיסמה ראשונית למתנדב
    //public void UpdatePassword(int id, string newPassword);  /// עדכון סיסמת המתנדב
    //public bool IsPasswordStrong(string password);// בדיקה אם הסיסמה חזקה מספיק
}
