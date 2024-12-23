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
    private const string FilePath = "call.xml";

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

    public void Create(Call item)
    {
        XElement callsRootElem = XMLTools.LoadListFromXMLElement(FilePath);
        if (callsRootElem.Elements().Any(c => (int?)c.Element("Id") == item.Id))
            throw new DalAlreadyExistsException($"Call with ID={item.Id} already exists");

        callsRootElem.Add(CreateElementFromCall(item));
        XMLTools.SaveListToXMLElement(callsRootElem, FilePath);
    }

    public Call? Read(int id)
    {
        XElement callsRootElem = XMLTools.LoadListFromXMLElement(FilePath);
        XElement? callElem = callsRootElem.Elements().FirstOrDefault(c => (int?)c.Element("Id") == id);
        return callElem is null ? null : CreateCallFromElement(callElem);
    }

    public Call? Read(Func<Call, bool> filter)
    {
        XElement callsRootElem = XMLTools.LoadListFromXMLElement(FilePath);
        return callsRootElem.Elements().Select(CreateCallFromElement).FirstOrDefault(filter);
    }

    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null)
    {
        XElement callsRootElem = XMLTools.LoadListFromXMLElement(FilePath);
        var calls = callsRootElem.Elements().Select(CreateCallFromElement);
        return filter == null ? calls : calls.Where(filter);
    }

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

    public void Delete(int id)
    {
        XElement callsRootElem = XMLTools.LoadListFromXMLElement(FilePath);
        XElement? callElem = callsRootElem.Elements().FirstOrDefault(c => (int?)c.Element("Id") == id);
        if (callElem is null)
            throw new DalDoesNotExistException($"Call with ID={id} does not exist");

        callElem.Remove();
        XMLTools.SaveListToXMLElement(callsRootElem, FilePath);
    }

    public void DeleteAll()
    {
        XMLTools.SaveListToXMLElement(new XElement("Calls"), FilePath);
    }

    public void Print(Call item)
    {
        Console.WriteLine($"Call ID: {item.Id}\nType: {item.CallType}\nAddress: {item.FullAddress}\nOpened: {item.OpenTime}\nEmergency: {item.isEmergency}\nDescription: {item.Description}\nLatitude: {item.Latitude}\nLongitude: {item.Longitude}\nMax Completion Time: {item.MaxCompletionTime}");
    }
}
