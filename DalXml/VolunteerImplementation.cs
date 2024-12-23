namespace Dal;

using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

internal class VolunteerImplementation : IVolunteer
{
    private const string FilePath = "volunteer.xml";

    public void Create(Volunteer item)
    {
        try
        {
            var volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(FilePath);
            if (volunteers.Any(v => v.Id == item.Id))
                throw new DalAlreadyExistsException($"Volunteer with ID={item.Id} already exists");

            volunteers.Add(item);
            Console.WriteLine("Saving new volunteer to XML...");
            XMLTools.SaveListToXMLSerializer(volunteers, FilePath);
            Console.WriteLine("Save successful.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during Create operation: {ex.Message}");
            throw;
        }
    }


    public Volunteer? Read(int id)
    {
        var volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(FilePath);
        return volunteers.FirstOrDefault(v => v.Id == id);
    }

    public Volunteer? Read(Func<Volunteer, bool> filter)
    {
        var volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(FilePath);
        return volunteers.FirstOrDefault(filter);
    }

    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null)
    {
        var volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(FilePath);
        return filter == null ? volunteers : volunteers.Where(filter);
    }

    public void Update(Volunteer item)
    {
        var volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(FilePath);
        if (volunteers.RemoveAll(v => v.Id == item.Id) == 0)
            throw new DalDoesNotExistException($"Volunteer with ID={item.Id} does not exist");

        volunteers.Add(item);
        XMLTools.SaveListToXMLSerializer(volunteers, FilePath);
    }

    public void Delete(int id)
    {
        var volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(FilePath);
        if (volunteers.RemoveAll(v => v.Id == id) == 0)
            throw new DalDoesNotExistException($"Volunteer with ID={id} does not exist");

        XMLTools.SaveListToXMLSerializer(volunteers, FilePath);
    }

    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(new List<Volunteer>(), FilePath);
    }

    public void Print(Volunteer item)
    {
        Console.WriteLine($"Volunteer ID: {item.Id}\nFull Name: {item.FullName}\nPhone: {item.Phone}\nEmail: {item.Email}\nRole: {item.Role}\nIs Active: {item.IsActive}\nMax Distance: {item.MaxDistance}\nDistance Type: {item.DistanceType}");
    }

    public void SetInitialPassword(int id, string password)
    {
        var volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(FilePath);
        var volunteer = volunteers.FirstOrDefault(v => v.Id == id) ?? throw new DalDoesNotExistException($"Volunteer with ID={id} does not exist");

        volunteer = volunteer with { Password = password };
        Update(volunteer);
    }

    public void UpdatePassword(int id, string newPassword)
    {
        var volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(FilePath);
        var volunteer = volunteers.FirstOrDefault(v => v.Id == id) ?? throw new DalDoesNotExistException($"Volunteer with ID={id} does not exist");

        volunteer = volunteer with { Password = newPassword };
        Update(volunteer);
    }

    public bool IsPasswordStrong(string password)
    {
        return password.Length >= 8 && password.Any(char.IsUpper) && password.Any(char.IsLower) && password.Any(char.IsDigit);
    }
}