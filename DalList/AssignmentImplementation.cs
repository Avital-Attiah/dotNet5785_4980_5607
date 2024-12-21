
namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;
using System.Linq;

internal class AssignmentImplementation : IAssignment
{
    // מתודת יצירה/הוספה Create
    public void Create(Assignment item)
    {
        // הוספה לרשימת ההקצאות
        DataSource.Assignments.Add(item);
    }


    // מתודת בקשה/קבלה של אובייקט בודד Read
    public Assignment? Read(int id)
    {
        return DataSource.Assignments.FirstOrDefault(item => item.Id == id);
    }

    // מתודת בקשה/קבלה של כל האובייקטים מטיפוס מסוים ReadAll
    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
    => filter == null
            ? DataSource.Assignments.Select(item => item)
            : DataSource.Assignments.Where(filter);

    // מתודת עדכון של אובייקט קיים Update
    public void Update(Assignment item)
    {
        // חיפוש הקצאה עם ה-ID המתאים
        var existingAssignment = DataSource.Assignments.FirstOrDefault(a => a.Id == item.Id);
        if (existingAssignment == null)
        {
            throw new DalDoesNotExistException($"Assignment with ID {item.Id} does not exist.");
        }
        // הסרה והוספה של האובייקט המעודכן
        DataSource.Assignments.Remove(existingAssignment);
        DataSource.Assignments.Add(item);
    }

    // מתודת מחיקה של אובייקט קיים Delete
    public void Delete(int id)
    {
        // חיפוש ההקצאה למחיקה
        var assignmentToDelete = DataSource.Assignments.FirstOrDefault(a => a.Id == id);
        if (assignmentToDelete == null)
        {
            throw new DalDoesNotExistException($"Assignment with ID {id} does not exist.");
        }
        // מחיקת ההקצאה מהרשימה
        DataSource.Assignments.Remove(assignmentToDelete);
    }

    // מתודת מחיקה של כל האובייקטים מטיפוס מסוים DeleteAll
    public void DeleteAll()
    {
        // ניקוי הרשימה כולה
        DataSource.Assignments.Clear();
    }

    public void Print(Assignment item)
    {
        Console.WriteLine("Id:"+item.Id+ " Call-Id:"+item.CallId+ " Volunteer-Id:"+item.VolunteerId+ " Entry-time:"+item.EntryTime+ " Completion-time:"+item.CompletionTime+ " Status:"+item.Status);
    }

    public Assignment? Read(Func<Assignment, bool> filter)
    {
        return DataSource.Assignments.FirstOrDefault(filter);
    }
}

