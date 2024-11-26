namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;

public class VolunteerImplementation : IVolunteer
{
    // מתודת יצירה/הוספה Create
    public void Create(Volunteer item)
    {
        // בדיקה אם קיים מתנדב עם אותו ID
        if (DataSource.Volunteers.Any(v => v.Id == item.Id))
        {
            throw new InvalidOperationException($"Volunteer with ID {item.Id} already exists.");
        }
        // הוספה לרשימת המתנדבים
        DataSource.Volunteers.Add(item);
    }

    // מתודת בקשה/קבלה של אובייקט בודד Read
    public Volunteer? Read(int id)
    {
        foreach (var volunteer in DataSource.Volunteers)
        {
            if (volunteer.Id == id)
            {
                return volunteer;
            }
        }
        return null;
    }

    // מתודת בקשה/קבלה של כל האובייקטים מטיפוס מסוים ReadAll
    public List<Volunteer> ReadAll()
    {
        // החזרת עותק של הרשימה הקיימת
        return new List<Volunteer>(DataSource.Volunteers);
    }

    // מתודת עדכון של אובייקט קיים Update
    public void Update(Volunteer item)
    {
        // חיפוש מתנדב עם ה-ID המתאים
        var existingVolunteer = DataSource.Volunteers.FirstOrDefault(v => v.Id == item.Id);
        if (existingVolunteer == null)
        {
            throw new InvalidOperationException($"Volunteer with ID {item.Id} does not exist.");
        }
        // הסרה והוספה של האובייקט המעודכן
        DataSource.Volunteers.Remove(existingVolunteer);
        DataSource.Volunteers.Add(item);
    }

    // מתודת מחיקה של אובייקט קיים Delete
    public void Delete(int id)
    {
        // חיפוש המתנדב למחיקה
        var volunteerToDelete = DataSource.Volunteers.FirstOrDefault(v => v.Id == id);
        if (volunteerToDelete == null)
        {
            throw new InvalidOperationException($"Volunteer with ID {id} does not exist.");
        }
        // מחיקת המתנדב מהרשימה
        DataSource.Volunteers.Remove(volunteerToDelete);
    }

    // מתודת מחיקה של כל האובייקטים מטיפוס מסוים DeleteAll
    public void DeleteAll()
    {
        // ניקוי הרשימה כולה
        DataSource.Volunteers.Clear();
    }

    public void Print(Volunteer item)
    {
        Console.WriteLine("Id:"+item.Id+ " Name:"+ item.FullName+" Phone:"+item.Phone+" Email:"+item.Email+" Password:"+item.Password+" Adress"+item.CurrentAddress+" Role:"+item.Role+ " IsActive:" + item.IsActive+ " Max distance" + item.MaxDistance+ " Distance-type" + item.DistanceType);
    }
 
}
