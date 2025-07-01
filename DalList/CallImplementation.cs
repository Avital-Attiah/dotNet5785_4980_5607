
namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

internal class CallImplementation : ICall
{
    // מתודת יצירה/הוספה Create
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Call item)
    {
        // הוספה לרשימת הקריאות
        DataSource.Calls.Add(item);
    }

    //Read מתודת בקשה/קבלה של אובייקט בודד 
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Call? Read(int id)
    {
        return DataSource.Calls.FirstOrDefault(item => item.Id == id);
       
    }

    // מתודת בקשה/קבלה של כל האובייקטים מטיפוס מסוים ReadAll
    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null)
           => filter == null
            ? DataSource.Calls.Select(item => item)
            : DataSource.Calls.Where(filter);


    // מתודת עדכון של אובייקט קיים Update
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Call item)
    {
        // חיפוש הקריאה עם ה-ID המתאים
        var existingCall = DataSource.Calls.FirstOrDefault(c => c.Id == item.Id);
        if (existingCall == null)
        {
            throw new DalDoesNotExistException($"Call with ID {item.Id} does not exist.");
        }
        // הסרה והוספה של האובייקט המעודכן
        DataSource.Calls.Remove(existingCall);
        DataSource.Calls.Add(item);
    }

    // מתודת מחיקה של אובייקט קיים Delete
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        // חיפוש הקריאה למחיקה
        var callToDelete = DataSource.Calls.FirstOrDefault(c => c.Id == id);
        if (callToDelete == null)
        {
            throw new DalDoesNotExistException($"Call with ID {id} does not exist.");
        }
        // מחיקת הקריאה מהרשימה
        DataSource.Calls.Remove(callToDelete);
    }

    // מתודת מחיקה של כל האובייקטים מטיפוס מסוים DeleteAll
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        // ניקוי הרשימה כולה
        DataSource.Calls.Clear();
    }

    public void Print(Call item)
    {
        Console.WriteLine("Id:"+item.Id+ " Call-type:"+item.CallType+ " Address:"+item.FullAddress+ " Open-time:"+item.OpenTime+ " Is-emergency:"+item.isEmergency+ " Description:"+item.Description+ " Max-completion-time:"+item.MaxCompletionTime);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public Call? Read(Func<Call, bool> filter)
    {
        return DataSource.Calls.FirstOrDefault(filter);
    }
}
