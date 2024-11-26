
namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;

public class AssignmentImplementation : IAssignment
{
    // מתודת יצירה/הוספה Create
    public void Create(Assignment item)
    {
        // בדיקה אם קיימת הקצאה עם אותו ID
        if (DataSource.Assignments.Any(a => a.Id == item.Id))
        {
            throw new InvalidOperationException($"Assignment with ID {item.Id} already exists.");
        }
        // הוספה לרשימת ההקצאות
        DataSource.Assignments.Add(item);
    }

    // מתודת בקשה/קבלה של אובייקט בודד Read
    public Assignment? Read(int id)
    {
        foreach (var assignment in DataSource.Assignments)
        {
            if (assignment.Id == id)
            {
                return assignment;
            }
        }
        return null;
    }

    // מתודת בקשה/קבלה של כל האובייקטים מטיפוס מסוים ReadAll
    public List<Assignment> ReadAll()
    {
        // החזרת עותק של הרשימה הקיימת
        return new List<Assignment>(DataSource.Assignments);
    }

    // מתודת עדכון של אובייקט קיים Update
    public void Update(Assignment item)
    {
        // חיפוש הקצאה עם ה-ID המתאים
        var existingAssignment = DataSource.Assignments.FirstOrDefault(a => a.Id == item.Id);
        if (existingAssignment == null)
        {
            throw new InvalidOperationException($"Assignment with ID {item.Id} does not exist.");
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
            throw new InvalidOperationException($"Assignment with ID {id} does not exist.");
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
}

