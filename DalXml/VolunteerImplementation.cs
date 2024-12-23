using DalApi;
using DO;
using System.ComponentModel;

namespace Dal;

internal class VolunteerImplementation : IVolunteer
{
    private const string FilePath = "volunteer.xml";

    // Create
    public void Create(Volunteer item)
    {
        if (Read(item.Id) is not null)
            throw new DalAlreadyExistsException($"Volunteer with ID={item.Id} already exists");

        volunteers.Add(item);
        XMLTools.SaveListToXMLSerializer(volunteers, FilePath);
    }

    // Delete
    public void Delete(int id)
    {
        Volunteer volunteer = Read(id);
        if (volunteer == null)
            throw new DalDoesNotExistException($"Volunteer with ID={id} does not exist");

        // טוען את המתנדבים מהקובץ
        List<Volunteer> volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(FilePath);

        // מסיר את המתנדב מהרשימה
        volunteers.Remove(volunteer);

        // שומר את הרשימה המעודכנת בקובץ
        XMLTools.SaveListToXMLSerializer(volunteers, FilePath);
    }

    // DeleteAll
    public void DeleteAll()
    {
        // טוען את המתנדבים מהקובץ
        List<Volunteer> volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(FilePath);

        // מנקה את כל המתנדבים
        volunteers.Clear();

        // שומר את הרשימה המעודכנת בקובץ
        XMLTools.SaveListToXMLSerializer(volunteers, FilePath);
    }

    public void Print(Volunteer item)
    {
        throw new NotImplementedException();
    }

    // Read (בפונקציה הראשונה לפי מזהה)
    public Volunteer? Read(int id)
    {
        // טוען את המתנדבים מהקובץ
        List<Volunteer> volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(FilePath);

        // מחפש את המתנדב לפי מזהה
        return volunteers.FirstOrDefault(v => v.Id == id);
    }

    // Read לפי פונקציה מותאמת אישית (למשל, לפי פילטר)
    public Volunteer? Read(Func<Volunteer, bool> filter)
    {
        // טוען את המתנדבים מהקובץ
        List<Volunteer> volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(FilePath);

        // מחפש את המתנדב לפי הפילטר
        return volunteers.FirstOrDefault(filter);
    }

    // ReadAll עם פילטר אופציונלי
    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null)
    {
        // טוען את המתנדבים מהקובץ
        List<Volunteer> volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(FilePath);

        // אם יש פילטר, מחזיר את המתנדבים המפולטרים, אחרת מחזיר את כל המתנדבים
        return filter == null
            ? new List<Volunteer>(volunteers)
            : new List<Volunteer>(volunteers.Where(filter));
    }

    // Update
    public void Update(Volunteer item)
    {
        // בודק אם המתנדב קיים
        Volunteer volunteer = Read(item.Id);
        if (volunteer == null)
            throw new DalDoesNotExistException($"Volunteer with ID={item.Id} does not exist");

        // מסיר את המתנדב הקיים ומוסיף את המתנדב המעודכן
        Delete(item.Id);
        Create(item);
    }
}
