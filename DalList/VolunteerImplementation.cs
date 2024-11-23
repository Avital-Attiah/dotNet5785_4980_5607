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
        return DataSource.Volunteers.FirstOrDefault(v => v.Id == id);
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

    //public void SetInitialPassword(int id, string password)
    //{
    //    var volunteer = Read(id);
    //    if (volunteer == null)
    //    {
    //        throw new InvalidOperationException($"Volunteer with ID {id} does not exist.");
    //    }

    //    if (!string.IsNullOrEmpty(volunteer.Password))
    //    {
    //        throw new InvalidOperationException("Initial password is already set.");
    //    }

    //    if (IsPasswordStrong(password))
    //    {
    //        // יצירת עותק חדש עם הסיסמה המעודכנת
    //        var updatedVolunteer = volunteer with { Password = password };
    //        Update(updatedVolunteer);
    //    }
    //    else
    //    {
    //        throw new InvalidOperationException("The initial password is not strong enough.");
    //    }
    //}

    //// מתודת עדכון סיסמה
    //public void UpdatePassword(int id, string newPassword)
    //{
    //    // שלב 1: קריאה למתנדב עם ה-ID המסוים
    //    var volunteer = Read(id);
    //    if (volunteer == null)
    //    {
    //        throw new InvalidOperationException($"Volunteer with ID {id} does not exist.");
    //    }

    //    // שלב 2: בדיקה אם הסיסמה חזקה
    //    if (!IsPasswordStrong(newPassword))
    //    {
    //        throw new InvalidOperationException("The new password is not strong enough.");
    //    }

    //    // שלב 3: יצירת עותק חדש של המתנדב עם הסיסמה המעודכנת
    //    var updatedVolunteer = volunteer with { Password = newPassword };

    //    // שלב 4: עדכון הנתונים עם האובייקט המעודכן
    //    Update(updatedVolunteer);
    //}


    //// בדיקה אם הסיסמא חזקה מספיק
    //public bool IsPasswordStrong(string password)
    //{
    //    if (password.Length < 8)
    //        return false;

    //    bool hasUpper = false, hasLower = false, hasDigit = false, hasSpecialChar = false;

    //    foreach (var c in password)
    //    {
    //        if (char.IsUpper(c)) hasUpper = true;
    //        else if (char.IsLower(c)) hasLower = true;
    //        else if (char.IsDigit(c)) hasDigit = true;
    //        else hasSpecialChar = true;
    //    }

    //    return hasUpper && hasLower && hasDigit && hasSpecialChar;
    //}
}
