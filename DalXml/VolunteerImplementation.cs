namespace Dal;

using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

internal class VolunteerImplementation : IVolunteer
{
    private const string FilePath = "volunteers.xml"; // Path to the XML file where volunteers are stored

    public void Create(Volunteer item)
    {
        try
        {
            var volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(FilePath); // Load existing volunteers
            if (volunteers.Any(v => v.Id == item.Id)) // Check if the volunteer already exists
                throw new DalAlreadyExistsException($"Volunteer with ID={item.Id} already exists");

            volunteers.Add(item); // Add the new volunteer
            Console.WriteLine("Saving new volunteer to XML...");
            XMLTools.SaveListToXMLSerializer(volunteers, FilePath); // Save the list to XML
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
        var volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(FilePath); // Load existing volunteers
        return volunteers.FirstOrDefault(v => v.Id == id); // Find the volunteer by ID
    }

    public Volunteer? Read(Func<Volunteer, bool> filter)
    {
        var volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(FilePath); // Load existing volunteers
        return volunteers.FirstOrDefault(filter); // Find the volunteer based on the filter
    }

    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null)
    {
        var volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(FilePath); // Load existing volunteers
        return filter == null ? volunteers : volunteers.Where(filter); // Return all or filtered volunteers
    }

    public void Update(Volunteer item)
    {
        var volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(FilePath); // Load existing volunteers
        if (volunteers.RemoveAll(v => v.Id == item.Id) == 0) // Remove volunteer with the same ID
            throw new DalDoesNotExistException($"Volunteer with ID={item.Id} does not exist");

        volunteers.Add(item); // Add the updated volunteer
        XMLTools.SaveListToXMLSerializer(volunteers, FilePath); // Save the list to XML
    }

    public void Delete(int id)
    {
        var volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(FilePath); // Load existing volunteers
        if (volunteers.RemoveAll(v => v.Id == id) == 0) // Remove volunteer by ID
            throw new DalDoesNotExistException($"Volunteer with ID={id} does not exist");

        XMLTools.SaveListToXMLSerializer(volunteers, FilePath); // Save the updated list to XML
    }

    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(new List<Volunteer>(), FilePath); // Clear the volunteer list in XML
    }

    public void Print(Volunteer item)
    {
        // Print volunteer details
        Console.WriteLine($"Volunteer ID: {item.Id}\nFull Name: {item.FullName}\nPhone: {item.Phone}\nEmail: {item.Email}\nRole: {item.Role}\nIs Active: {item.IsActive}\nMax Distance: {item.MaxDistance}\nDistance Type: {item.DistanceType}");
    }

    public void SetInitialPassword(int id, string password)
    {
        var volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(FilePath); // Load existing volunteers
        var volunteer = volunteers.FirstOrDefault(v => v.Id == id) ?? throw new DalDoesNotExistException($"Volunteer with ID={id} does not exist");

        volunteer = volunteer with { Password = password }; // Set the initial password
        Update(volunteer); // Update the volunteer's details
    }

    public void UpdatePassword(int id, string newPassword)
    {
        var volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(FilePath); // Load existing volunteers
        var volunteer = volunteers.FirstOrDefault(v => v.Id == id) ?? throw new DalDoesNotExistException($"Volunteer with ID={id} does not exist");

        volunteer = volunteer with { Password = newPassword }; // Update the password
        Update(volunteer); // Update the volunteer's details
    }

    public bool IsPasswordStrong(string password)
    {
        // Check if the password meets strength requirements
        return password.Length >= 8 && password.Any(char.IsUpper) && password.Any(char.IsLower) && password.Any(char.IsDigit);
    }
}
