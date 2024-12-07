namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;

internal class VolunteerImplementation : IVolunteer
{
    // מתודת יצירה/הוספה Create
    public void Create(Volunteer item)
    {
        // בדיקה אם קיים מתנדב עם אותו ID
        if (DataSource.Volunteers.Any(v => v.Id == item.Id))
        {
            throw new DalAlreadyExistsException($"Volunteer with ID {item.Id} already exists.");
        }
        // הוספה לרשימת המתנדבים
        DataSource.Volunteers.Add(item);
    }

    // מתודת בקשה/קבלה של אובייקט בודד Read
    public Volunteer? Read(int id)
    {
        return DataSource.Volunteers.FirstOrDefault(item => item.Id == id);
    }

    // מתודת בקשה/קבלה של כל האובייקטים מטיפוס מסוים ReadAll
    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null)
          => filter == null
            ? DataSource.Volunteers.Select(item => item)
            : DataSource.Volunteers.Where(filter);


    

// מתודת עדכון של אובייקט קיים Update
public void Update(Volunteer item)
    {
        // חיפוש מתנדב עם ה-ID המתאים
        var existingVolunteer = DataSource.Volunteers.FirstOrDefault(v => v.Id == item.Id);
        if (existingVolunteer == null)
        {
            throw new DalDoesNotExistException($"Volunteer with ID {item.Id} does not exist.");
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
            throw new DalDoesNotExistException($"Volunteer with ID {id} does not exist.");
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

    public Volunteer? Read(Func<Volunteer, bool> filter)
    {
        return DataSource.Volunteers.FirstOrDefault(filter);
    }
}
