
namespace DalApi;
using DO;
public interface IVolunteer
{
    void Create(Volunteer item); //Creates new entity object in DAL
    Volunteer? Read(int id); //Reads entity object by its ID 
    List<Volunteer> ReadAll(); //stage 1 only, Reads all entity objects
    void Update(Volunteer item); //Updates entity object
    void Delete(int id); //Deletes an object by its Id
    void DeleteAll(); //Delete all entity objects
    void Print(Volunteer item);
    //public void SetInitialPassword(int id, string password);/// יצירת סיסמה ראשונית למתנדב
    //public void UpdatePassword(int id, string newPassword);  /// עדכון סיסמת המתנדב
    //public bool IsPasswordStrong(string password);//בדיקה אם הסיסמא חזקה מספיק
}
