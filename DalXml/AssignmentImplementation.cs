namespace Dal;

using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using static DO.Enums;

internal class AssignmentImplementation : IAssignment
{
    private const string FilePath = Config.s_assignments_xml;

    public void Create(Assignment item)
    {
        var assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(FilePath);
        if (assignments.Any(a => a.Id == item.Id))
            throw new DalAlreadyExistsException($"Assignment with ID={item.Id} already exists");

        assignments.Add(item);
        XMLTools.SaveListToXMLSerializer(assignments, FilePath);
    }

    public Assignment? Read(int id)
    {
        var assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(FilePath);
        return assignments.FirstOrDefault(a => a.Id == id);
    }

    public Assignment? Read(Func<Assignment, bool> filter)
    {
        var assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(FilePath);
        return assignments.FirstOrDefault(filter);
    }

    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
    {
        var assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(FilePath);
        return filter == null ? assignments : assignments.Where(filter);
    }

    public void Update(Assignment item)
    {
        var assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(FilePath);
        if (assignments.RemoveAll(a => a.Id == item.Id) == 0)
            throw new DalDoesNotExistException($"Assignment with ID={item.Id} does not exist");

        assignments.Add(item);
        XMLTools.SaveListToXMLSerializer(assignments, FilePath);
    }

    public void Delete(int id)
    {
        var assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(FilePath);
        if (assignments.RemoveAll(a => a.Id == id) == 0)
            throw new DalDoesNotExistException($"Assignment with ID={id} does not exist");

        XMLTools.SaveListToXMLSerializer(assignments, FilePath);
    }

    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(new List<Assignment>(), FilePath);
    }

    public void Print(Assignment item)
    {
        Console.WriteLine($"Assignment ID: {item.Id}\nCall ID: {item.CallId}\nVolunteer ID: {item.VolunteerId}\nEntry Time: {item.EntryTime}\nCompletion Time: {item.CompletionTime}\nStatus: {item.Status}");
    }

    // LINQ to XML for demonstration purposes
    public Assignment? ReadWithXElement(int id)
    {
        XElement? assignmentElem = XMLTools.LoadListFromXMLElement(FilePath).Elements().FirstOrDefault(a => (int?)a.Element("Id") == id);
        return assignmentElem == null ? null : GetAssignmentFromXElement(assignmentElem);
    }

    public void UpdateWithXElement(Assignment item)
    {
        XElement assignmentsRoot = XMLTools.LoadListFromXMLElement(FilePath);

        XElement? assignmentElem = assignmentsRoot.Elements().FirstOrDefault(a => (int?)a.Element("Id") == item.Id);
        if (assignmentElem == null)
            throw new DalDoesNotExistException($"Assignment with ID={item.Id} does not exist");

        assignmentElem.ReplaceWith(CreateXElementFromAssignment(item));
        XMLTools.SaveListToXMLElement(assignmentsRoot, FilePath);
    }

    private static Assignment GetAssignmentFromXElement(XElement elem)
    {
        return new Assignment
        (
            Id: (int?)elem.Element("Id") ?? throw new FormatException("Missing ID"),
            CallId: (int?)elem.Element("CallId") ?? throw new FormatException("Missing CallId"),
            VolunteerId: (int?)elem.Element("VolunteerId") ?? throw new FormatException("Missing VolunteerId"),
            EntryTime: DateTime.Parse((string?)elem.Element("EntryTime") ?? throw new FormatException("Missing EntryTime")),
            CompletionTime: DateTime.TryParse((string?)elem.Element("CompletionTime"), out var completionTime) ? completionTime : null,
            Status: Enum.TryParse((string?)elem.Element("Status"), out TreatmentStatus status) ? status : null
        );
    }

    private static XElement CreateXElementFromAssignment(Assignment assignment)
    {
        return new XElement("Assignment",
            new XElement("Id", assignment.Id),
            new XElement("CallId", assignment.CallId),
            new XElement("VolunteerId", assignment.VolunteerId),
            new XElement("EntryTime", assignment.EntryTime),
            new XElement("CompletionTime", assignment.CompletionTime),
            new XElement("Status", assignment.Status));
    }
}
