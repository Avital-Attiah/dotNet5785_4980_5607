namespace BlImplementation;
using BlApi;
using BO;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

internal class CallImplementation : ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    public void Create(BO.Call boCall)
    {
        // Perform validation
        CallManager.ValidateCall(boCall);

        // Create DO.Call object
        DO.Call doCall = new(boCall.Id,(DO.Enums.CallType)boCall.CallType, boCall.FullAddress, boCall.OpenTime,false, boCall.Description, boCall.Latitude, boCall.Longitude, 
        boCall.MaxCompletionTime);

        try
        {
            _dal.Call.Create(doCall);
        }
        catch (DO.DalAlreadyExistsException ex)
        {
            throw new BO.BlDoesNotExistException($"Call with ID={boCall.Id} already exists", ex);
        }
    }

    public void CancellationOfTreatment(int CancellerId, int assignmentId)
    {
        DO.Assignment? doAssignment = _dal.Assignment.Read(assignmentId);
        DO.Volunteer doVolunteer = _dal.Volunteer.Read(CancellerId)!;
        try
        {
            if (doAssignment == null)
                throw new BO.BlDoesNotExistException($"Assignment with ID={assignmentId} does not exist.");
            if (doAssignment.VolunteerId != CancellerId && doVolunteer.Role != DO.Enums.Role.Manager || doAssignment.CompletionTime != null || doAssignment.Status != null)
            {
                throw new BO.InvalidAssignmentCompletionException($"Assignment with ID={assignmentId} cannot be completed.");
            }
            else
            {
                DO.Enums.TreatmentStatus finishType = doVolunteer.Role == DO.Enums.Role.Volunteer ? DO.Enums.TreatmentStatus.CanceledByVolunteer : DO.Enums.TreatmentStatus.CanceledByManager;
                DO.Assignment copyAssignment = doAssignment with { CompletionTime = ClockManager.Now, Status = finishType };
                _dal.Assignment.Update(copyAssignment);
            }
        }
        catch (BO.BlDoesNotExistException ex)
        {
            throw;
        }
        catch (BO.InvalidAssignmentCompletionException ex)
        {
            throw;
        }
    }

    public void DeleteCall(int callId)
    {
        try
        {
            var doCall = _dal.Call.Read(callId)
                            ?? throw new BO.BlDoesNotExistException($"Call with ID={callId} does not exist.");

            var statusCall = CallManager.GetStatusCall(callId);

            if (statusCall != BO.CallStatus.Open)
            {
                throw new BO.BlCannotBeDeletedException("Call must be in Open status to be deleted.");
            }

            var assignment = _dal.Assignment.Read(a => a.CallId == callId);
            if (assignment != null)
            {
                throw new BO.BlCannotBeDeletedException("Call has been assigned to a volunteer and cannot be deleted.");
            }

            _dal.Call.Delete(callId);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Call with ID={callId} does not exist in the data layer.", ex);
        }
        catch (BO.BlCannotBeDeletedException)
        {
            throw;
        }
    }

    public void FinishCall(int volunteerId, int assignmentId)
    {
        DO.Assignment? doAssignment = _dal.Assignment.Read(assignmentId);
        try
        {
            if (doAssignment == null)
                throw new BO.BlDoesNotExistException($"Assignment with ID={assignmentId} does not exist.");
            if (doAssignment.VolunteerId != volunteerId || doAssignment.Status != null || doAssignment.CompletionTime != null)
            {
                throw new BO.InvalidAssignmentCompletionException($"Assignment with ID={assignmentId} cannot be completed.");
            }
            else
            {
                DO.Assignment copyAssignment = doAssignment with { CompletionTime = ClockManager.Now, Status = DO.Enums.TreatmentStatus.CompletedOnTime };
                _dal.Assignment.Update(copyAssignment);
            }
        }
        catch (BO.BlDoesNotExistException ex)
        {
            throw;
        }
        catch (BO.InvalidAssignmentCompletionException ex)
        {
            throw;
        }
    }

    public int[] GetCallCounts()
    {
        var callList = _dal.Call.ReadAll();
        var statusValues = Enum.GetValues(typeof(BO.CallStatus)).Cast<BO.CallStatus>();
        int[] callCounts = new int[statusValues.Count()];

        var groupedCalls = callList
            .GroupBy(item => CallManager.GetStatusCall(item.Id))
            .ToDictionary(group => group.Key, group => group.Count());


        foreach (var status in statusValues)
        {
            callCounts[(int)status] = groupedCalls.GetValueOrDefault(status, 0);
        }

        return callCounts;
    }



    public BO.Call? Read(int callId)
    {

        var doCall = _dal.Call.Read(callId) ??
        throw new BO.BlDoesNotExistException($"Call with ID={callId} does Not exist");
        return new()
        {
            Id = callId,
            FullAddress = doCall.FullAddress,
            Description = doCall.Description,
            OpenTime = doCall.OpenTime,
            Latitude = doCall.Latitude,
            Longitude = doCall.Longitude,
            MaxCompletionTime = doCall.MaxCompletionTime,
            CallType = (BO.CallType)doCall.CallType,
            Status = CallManager.GetStatusCall(callId),
            ListAssignments = CallManager.GetAssignmentsHistory(callId)
        };
    }


    public IEnumerable<BO.CallInList> GetCallsList(BO.CallInListFieldSor? filterByField = null, object? value = null, BO.CallInListFieldSor? sortByField = null)
    {
        var doCalls = _dal.Call.ReadAll();
        var callsInList = doCalls.Select(item =>
        {
            var doAssignment = _dal.Assignment.Read(a => a.CallId == item.Id);
            TimeSpan? remainingTime = item.MaxCompletionTime.HasValue
            ? item.MaxCompletionTime.Value - ClockManager.Now
            : (TimeSpan?)null;
            string? volunteerName = doAssignment != null ? _dal.Volunteer.Read(doAssignment.VolunteerId)!.FullName : null;
            TimeSpan? completionTime = doAssignment != null && doAssignment.Status != null
            ? doAssignment.CompletionTime - doAssignment.EntryTime : null;
            return new BO.CallInList
            {
                Id = doAssignment == null ? null : doAssignment.Id,
                CallId = item.Id,
                CallType = (BO.CallType)item.CallType,
                OpenTime = item.OpenTime,
                TimeRemaining = remainingTime,
                LastAssignedVolunteerName = volunteerName,
                TimeToComplete = completionTime,
                Status = CallManager.GetStatusCall(item.Id),
                AssignmentsCount = _dal.Assignment.ReadAll(a => a.CallId == item.Id).Count()
            };
        })
        .ToList();
        if (filterByField != null && value != null)
        {
            callsInList = Tools.FilterList(callsInList, filterByField, value).ToList();
        }
       
        if (sortByField != null)
        {
            callsInList = Tools.SortByEnum<BO.CallInList, BO.CallInListFieldSor>(callsInList, sortByField);
        }
        else
        {
            callsInList = callsInList.OrderByDescending(v => v.Id).ToList();
        }

        return callsInList;
    }



    public IEnumerable<BO.ClosedCallInList> GetClosedCallsByVolunteer(int volunteerId, BO.CallType? callTypeFilter = null, BO.ClosedCallInListFields? sortField = null)
    {
        var doCallList = (from assignment in _dal.Assignment.ReadAll(a => a.VolunteerId == volunteerId)
                          let call = _dal.Call.Read(c => c.Id == assignment.CallId && assignment.CompletionTime != null)
                          where call != null
                          select call)
                  .ToList();
        var closedCallsInList = doCallList.Select(item =>
        {
            var doAssignment = _dal.Assignment.Read(a => a.CallId == item.Id);
            return new BO.ClosedCallInList
            {
                Id = item.Id,
                CallType = (BO.CallType)item.CallType,
                FullAddress = item.FullAddress,
                OpenTime = item.OpenTime,
                StartHandlingTime = doAssignment!.EntryTime,
                EndHandlingTime = doAssignment.CompletionTime,
                ClosureType = (BO.ClosureType)doAssignment.Status!
            };
        })
            .ToList();
        if (callTypeFilter != null)
        {
            closedCallsInList = closedCallsInList.Where(item => item.CallType == callTypeFilter)
                .ToList();
        }

        if (sortField != null)
        {
            closedCallsInList = Tools.SortByEnum(closedCallsInList, sortField);
        }
        return closedCallsInList;
    }

    public IEnumerable<BO.OpenCallInList> GetOpenCalls(int volunteerId, BO.CallType? callTypeFilter = null, BO.OpenCallInListFields? sortField = null)
    {
        var doCallList = _dal.Call.ReadAll(c =>
            CallManager.GetStatusCall(c.Id) == BO.CallStatus.Open ||
            CallManager.GetStatusCall(c.Id) == BO.CallStatus.OpenAtRisk);
        string volunteerDistance = _dal.Volunteer.Read(volunteerId)!.CurrentAddress!;

        var openCallInList = doCallList.Select(item =>
        {
            return new BO.OpenCallInList
            {
                Id = item.Id,
                CallType = (BO.CallType)item.CallType,
                Description = item.Description,
                FullAddress = item.FullAddress,
                OpenTime = item.OpenTime,
                MaxCompletionTime = item.MaxCompletionTime,
                DistanceFromVolunteer = CallManager.GetDistanceBetweenAddresses(volunteerDistance, item.FullAddress)
            };
        }

        ).ToList();

        if (callTypeFilter != null)
        {
            openCallInList = openCallInList.Where(item => item.CallType == callTypeFilter)
                .ToList();
        }

        if (sortField != null)
        {
            openCallInList = Tools.SortByEnum(openCallInList, sortField);
        }
        return openCallInList;
    }

    public void SelectCall(int volunteerId, int callId)
    {

        DO.Call? doCall = _dal.Call.Read(callId);
        DO.Assignment? doAssignment = _dal.Assignment.Read(a => a.CallId == callId);
        try
        {
            if (doAssignment != null || CallManager.GetStatusCall(callId) == BO.CallStatus.InProgress
                || CallManager.GetStatusCall(callId) == BO.CallStatus.InProgressAtRisk
                || doCall.MaxCompletionTime != null && doCall.MaxCompletionTime >= ClockManager.Now)
            {
                throw new BO.InvalidCallSelectionException($"Call with ID={callId} cannot be selected.");
            }
            else
            {
                DO.Assignment newAssignment = new DO.Assignment()
                {
                    CallId = callId,
                    VolunteerId = volunteerId,
                    EntryTime = ClockManager.Now
                };
                _dal.Assignment.Create(newAssignment);
            }
        }
        catch (BO.InvalidCallSelectionException ex)
        {
            throw;
        }
    }


    public void UpdateCallDetails(BO.Call updateCallObj)
    {
        var doCall = _dal.Call.Read(updateCallObj.Id)
                      ?? throw new BO.BlDoesNotExistException($"Call with ID={updateCallObj.Id} does not exist");
        try
        {
            // Validate call details before updating
            CallManager.ValidateCall(updateCallObj, isUpdate: true);
            var copyDoCall = CallManager.updateIfNeededDoCall(doCall, updateCallObj);
            _dal.Call.Update(copyDoCall);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlCannotUpdateException($"Failed to update call with ID={doCall.Id}.", ex);
        }
        catch (BO.BlValidationException)
        {
            throw;
        }
        catch (BO.BlNullPropertyException)
        {
            throw;
        }
    }


}