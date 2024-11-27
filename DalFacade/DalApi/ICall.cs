
namespace DalApi;
using DO;
public interface ICall
{
    void Create(Call item); // יוצר אובייקט ישות חדש ב-DAL
    Call? Read(int id); // קורא אובייקט ישות לפי מזהה שלו
    List<Call> ReadAll(); // שלב 1 בלבד, קורא את כל אובייקטי הישות
    void Update(Call item); // מעדכן אובייקט ישות
    void Delete(int id); // מוחק אובייקט לפי המזהה שלו
    void DeleteAll(); // מוחק את כל אובייקטי הישות
    void Print(Call item); // מדפיס את הפריט
}