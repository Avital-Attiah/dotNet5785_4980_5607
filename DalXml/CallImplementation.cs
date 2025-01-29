namespace Dal;

using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using static DO.Enums;

internal class CallImplementation : ICall
{
    private const string FilePath = "calls.xml";

    // Converts an XElement to a Call object
    private static Call CreateCallFromElement(XElement element)
    {
        return new Call(
            Id: (int?)element.Element("Id") ?? throw new FormatException("Invalid ID"),
            CallType: Enum.TryParse(element.Element("CallType")?.Value, out CallType type) ? type : throw new FormatException("Invalid CallType"),
            FullAddress: element.Element("FullAddress")?.Value ?? throw new FormatException("Invalid FullAddress"),
            OpenTime: DateTime.Parse(element.Element("OpenTime")?.Value ?? throw new FormatException("Invalid OpenTime")),
            isEmergency: (bool?)element.Element("isEmergency") ?? false,
            Description: element.Element("Description")?.Value,
            Latitude: (double?)element.Element("Latitude") ?? 0.0,
            Longitude: (double?)element.Element("Longitude") ?? 0.0,
            MaxCompletionTime: DateTime.TryParse(element.Element("MaxCompletionTime")?.Value, out var maxTime) ? maxTime : null
        );
    }

    // Converts a Call object to an XElement
    private static XElement CreateElementFromCall(Call call)
    {
        return new XElement("Call",
            new XElement("Id", call.Id),
            new XElement("CallType", call.CallType),
            new XElement("FullAddress", call.FullAddress),
            new XElement("OpenTime", call.OpenTime),
            new XElement("isEmergency", call.isEmergency),
            new XElement("Description", call.Description),
            new XElement("Latitude", call.Latitude),
            new XElement("Longitude", call.Longitude),
            new XElement("MaxCompletionTime", call.MaxCompletionTime)
        );
    }

    // Adds a new Call to the XML file
    public void Create(Call item)
    {
        XElement callsRootElem = XMLTools.LoadListFromXMLElement(FilePath);
        int id = Config.NextCallId;
        Call copy = item with { Id = id };
        callsRootElem.Add(CreateElementFromCall(copy));
        XMLTools.SaveListToXMLElement(callsRootElem, FilePath);
    }

    // Reads a Call by ID from the XML file
    public Call? Read(int id)
    {
        XElement callsRootElem = XMLTools.LoadListFromXMLElement(FilePath);
        XElement? callElem = callsRootElem.Elements().FirstOrDefault(c => (int?)c.Element("Id") == id);
        return callElem is null ? null : CreateCallFromElement(callElem);
    }

    // Reads the first Call that matches the given filter
    public Call? Read(Func<Call, bool> filter)
    {
        XElement callsRootElem = XMLTools.LoadListFromXMLElement(FilePath);
        return callsRootElem.Elements().Select(CreateCallFromElement).FirstOrDefault(filter);
    }

    // Reads all Calls from the XML file, optionally filtering them
    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null)
    {
        XElement callsRootElem = XMLTools.LoadListFromXMLElement(FilePath);
        var calls = callsRootElem.Elements().Select(CreateCallFromElement);
        return filter == null ? calls : calls.Where(filter);
    }

    // Updates an existing Call in the XML file
    public void Update(Call item)
    {
        XElement callsRootElem = XMLTools.LoadListFromXMLElement(FilePath);
        XElement? callElem = callsRootElem.Elements().FirstOrDefault(c => (int?)c.Element("Id") == item.Id);
        if (callElem is null)
            throw new DalDoesNotExistException($"Call with ID={item.Id} does not exist");

        callElem.Remove();
        callsRootElem.Add(CreateElementFromCall(item));
        XMLTools.SaveListToXMLElement(callsRootElem, FilePath);
    }

    // Deletes a Call by ID from the XML file
    public void Delete(int id)
    {
        XElement callsRootElem = XMLTools.LoadListFromXMLElement(FilePath);
        XElement? callElem = callsRootElem.Elements().FirstOrDefault(c => (int?)c.Element("Id") == id);
        if (callElem is null)
            throw new DalDoesNotExistException($"Call with ID={id} does not exist");

        callElem.Remove();
        XMLTools.SaveListToXMLElement(callsRootElem, FilePath);
    }

    // Deletes all Calls from the XML file
    public void DeleteAll()
    {
        XMLTools.SaveListToXMLElement(new XElement("Calls"), FilePath);
    }

    // Prints Call details to the console
    public void Print(Call item)
    {
        Console.WriteLine($"Call ID: {item.Id}\nType: {item.CallType}\nAddress: {item.FullAddress}\nOpened: {item.OpenTime}\nEmergency: {item.isEmergency}\nDescription: {item.Description}\nLatitude: {item.Latitude}\nLongitude: {item.Longitude}\nMax Completion Time: {item.MaxCompletionTime}");
    }
}
