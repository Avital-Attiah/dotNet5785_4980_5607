using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalApi;

public interface ICrud<T> where T : class 
{
    void Create(T item); // יוצר אובייקט ישות חדש ב-DAL
    T? Read(int id); // קורא אובייקט ישות לפי מזהה שלו
    List<T> ReadAll(); // שלב 1 בלבד, קורא את כל אובייקטי הישות
    void Update(T item); // מעדכן אובייקט ישות
    void Delete(int id); // מוחק אובייקט לפי המזהה שלו
    void DeleteAll(); // מוחק את כל אובייקטי הישות
}
