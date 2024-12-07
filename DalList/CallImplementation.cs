
namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;

internal class CallImplementation : ICall
{
    // מתודת יצירה/הוספה Create
    public void Create(Call item)
    {
        //  ID בדיקה אם קיימת קריאה עם אותו 
        if (DataSource.Calls.Any(c => c.Id == item.Id))
        {
            throw new InvalidOperationException($"Call with ID {item.Id} already exists.");
        }
        // הוספה לרשימת הקריאות
        DataSource.Calls.Add(item);
    }

    //Read מתודת בקשה/קבלה של אובייקט בודד 
    public Call? Read(int id)
    {
        foreach (var call in DataSource.Calls)
        {
            if (call.Id == id)
            {
                return call;
            }
        }
        return null;
    }

    // מתודת בקשה/קבלה של כל האובייקטים מטיפוס מסוים ReadAll
    public List<Call> ReadAll()
    {
        // החזרת עותק של הרשימה הקיימת
        return new List<Call>(DataSource.Calls);
    }

    // מתודת עדכון של אובייקט קיים Update
    public void Update(Call item)
    {
        // חיפוש הקריאה עם ה-ID המתאים
        var existingCall = DataSource.Calls.FirstOrDefault(c => c.Id == item.Id);
        if (existingCall == null)
        {
            throw new InvalidOperationException($"Call with ID {item.Id} does not exist.");
        }
        // הסרה והוספה של האובייקט המעודכן
        DataSource.Calls.Remove(existingCall);
        DataSource.Calls.Add(item);
    }

    // מתודת מחיקה של אובייקט קיים Delete
    public void Delete(int id)
    {
        // חיפוש הקריאה למחיקה
        var callToDelete = DataSource.Calls.FirstOrDefault(c => c.Id == id);
        if (callToDelete == null)
        {
            throw new InvalidOperationException($"Call with ID {id} does not exist.");
        }
        // מחיקת הקריאה מהרשימה
        DataSource.Calls.Remove(callToDelete);
    }

    // מתודת מחיקה של כל האובייקטים מטיפוס מסוים DeleteAll
    public void DeleteAll()
    {
        // ניקוי הרשימה כולה
        DataSource.Calls.Clear();
    }

    public void Print(Call item)
    {
        Console.WriteLine("Id:"+item.Id+ " Call-type:"+item.CallType+ " Address:"+item.FullAddress+ " Open-time:"+item.OpenTime+ " Is-emergency:"+item.isEmergency+ " Description:"+item.Description+ " Max-completion-time:"+item.MaxCompletionTime);
    }
}
