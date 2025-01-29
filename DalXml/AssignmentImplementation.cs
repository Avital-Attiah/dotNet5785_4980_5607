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
    private const string FilePath = "assignments.xml";

    /// <summary>
    /// Creates a new assignment and saves it to the XML file.
    /// </summary>
    /// <param name="item">The assignment object to create.</param>
    public void Create(Assignment item)
    {
        XElement assignmentElements = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml);

        // Retrieves the next available ID and updates the config file
        int id = Config.NextAssignmentId;

        // Creates a new copy of the Assignment with the new ID
        Assignment copy = item with { Id = id };

        // Adds the new XElement to the list
        assignmentElements.Add(GetXElement(copy));

        // Saves the updates to the XML file
        XMLTools.SaveListToXMLElement(assignmentElements, Config.s_assignments_xml);
    }

    /// <summary>
    /// Converts an Assignment object into an XElement representation for XML storage.
    /// </summary>
    /// <param name="assignment">The assignment object to convert.</param>
    /// <returns>An XElement representation of the assignment.</returns>
    static XElement GetXElement(Assignment assignment)
    {
        var elements = new List<XElement>();

        if (assignment.Id != 0)
            elements.Add(new XElement("Id", assignment.Id));

        if (assignment.CallId != 0)
            elements.Add(new XElement("CallId", assignment.CallId));

        if (assignment.VolunteerId != 0)
            elements.Add(new XElement("VolunteerId", assignment.VolunteerId));

        if (assignment.EntryTime != null)
            elements.Add(new XElement("EntryTime", assignment.EntryTime));

        if (assignment.CompletionTime != null)
            elements.Add(new XElement("CompletionTime", assignment.CompletionTime));

        if (assignment.Status != null)
            elements.Add(new XElement("Status", assignment.Status.ToString()));

        return new XElement("Assignment", elements);
    }

    /// <summary>
    /// Reads an assignment by its unique ID.
    /// </summary>
    /// <param name="id">The ID of the assignment to read.</param>
    /// <returns>The assignment object if found; otherwise, null.</returns>
    public Assignment? Read(int id)
    {
        var assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(FilePath);
        return assignments.FirstOrDefault(a => a.Id == id);
    }

    /// <summary>
    /// Reads an assignment based on a filter condition.
    /// </summary>
    /// <param name="filter">A function that defines the condition for retrieving an assignment.</param>
    /// <returns>The first assignment that matches the condition, or null if none found.</returns>
    public Assignment? Read(Func<Assignment, bool> filter)
    {
        var assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(FilePath);
        return assignments.FirstOrDefault(filter);
    }

    /// <summary>
    /// Reads all assignments that match a given filter (if provided).
    /// </summary>
    /// <param name="filter">An optional filter function.</param>
    /// <returns>A list of assignments that match the condition.</returns>
    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
    {
        var assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(FilePath);
        return filter == null ? assignments : assignments.Where(filter);
    }

    /// <summary>
    /// Updates an existing assignment.
    /// </summary>
    /// <param name="item">The updated assignment object.</param>
    /// <exception cref="DalDoesNotExistException">Thrown if the assignment does not exist.</exception>
    public void Update(Assignment item)
    {
        var assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(FilePath);
        if (assignments.RemoveAll(a => a.Id == item.Id) == 0)
            throw new DalDoesNotExistException($"Assignment with ID={item.Id} does not exist");

        assignments.Add(item);
        XMLTools.SaveListToXMLSerializer(assignments, FilePath);
    }

    /// <summary>
    /// Deletes an assignment by ID.
    /// </summary>
    /// <param name="id">The ID of the assignment to delete.</param>
    /// <exception cref="DalDoesNotExistException">Thrown if the assignment does not exist.</exception>
    public void Delete(int id)
    {
        var assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(FilePath);
        if (assignments.RemoveAll(a => a.Id == id) == 0)
            throw new DalDoesNotExistException($"Assignment with ID={id} does not exist");

        XMLTools.SaveListToXMLSerializer(assignments, FilePath);
    }

    /// <summary>
    /// Deletes all assignments from the XML file.
    /// </summary>
    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(new List<Assignment>(), FilePath);
    }

    /// <summary>
    /// Prints assignment details to the console.
    /// </summary>
    /// <param name="item">The assignment object to print.</param>
    public void Print(Assignment item)
    {
        Console.WriteLine($"Assignment ID: {item.Id}\nCall ID: {item.CallId}\nVolunteer ID: {item.VolunteerId}\nEntry Time: {item.EntryTime}\nCompletion Time: {item.CompletionTime}\nStatus: {item.Status}");
    }

    /// <summary>
    /// Reads an assignment using LINQ to XML.
    /// </summary>
    /// <param name="id">The ID of the assignment.</param>
    /// <returns>The assignment object if found; otherwise, null.</returns>
    public Assignment? ReadWithXElement(int id)
    {
        XElement? assignmentElem = XMLTools.LoadListFromXMLElement(FilePath).Elements().FirstOrDefault(a => (int?)a.Element("Id") == id);
        return assignmentElem == null ? null : GetAssignmentFromXElement(assignmentElem);
    }

    /// <summary>
    /// Updates an assignment using XElement.
    /// </summary>
    /// <param name="item">The updated assignment object.</param>
    /// <exception cref="DalDoesNotExistException">Thrown if the assignment does not exist.</exception>
    public void UpdateWithXElement(Assignment item)
    {
        XElement assignmentsRoot = XMLTools.LoadListFromXMLElement(FilePath);

        XElement? assignmentElem = assignmentsRoot.Elements().FirstOrDefault(a => (int?)a.Element("Id") == item.Id);
        if (assignmentElem == null)
            throw new DalDoesNotExistException($"Assignment with ID={item.Id} does not exist");

        assignmentElem.ReplaceWith(CreateXElementFromAssignment(item));
        XMLTools.SaveListToXMLElement(assignmentsRoot, FilePath);
    }

    /// <summary>
    /// Converts an XElement into an Assignment object.
    /// </summary>
    /// <param name="elem">The XElement to convert.</param>
    /// <returns>The corresponding Assignment object.</returns>
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

    /// <summary>
    /// Converts an Assignment object into an XElement representation.
    /// </summary>
    /// <param name="assignment">The assignment object to convert.</param>
    /// <returns>An XElement representing the assignment.</returns>
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
