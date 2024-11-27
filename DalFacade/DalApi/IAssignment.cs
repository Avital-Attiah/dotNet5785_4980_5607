
namespace DalApi;
using DO;
public interface IAssignment
{
    void Create(Assignment item); // יוצר אובייקט ישות חדש ב-DAL
    Assignment? Read(int id); // קורא אובייקט ישות לפי מזהה שלו
    List<Assignment> ReadAll(); // שלב 1 בלבד, קורא את כל אובייקטי הישות
    void Update(Assignment item); // מעדכן אובייקט ישות
    void Delete(int id); // מוחק אובייקט לפי המזהה שלו
    void DeleteAll(); // מוחק את כל אובייקטי הישות
    void Print(Assignment item); // מדפיס את הפריט
}


