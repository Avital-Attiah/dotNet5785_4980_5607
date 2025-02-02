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

    // Create a new call
    public void Create(BO.Call boCall)
    {
        // Validate the call before creation
        CallManager.ValidateCall(boCall);

        // Create a DO.Call object from the BO.Call object
        DO.Call doCall = new(boCall.Id, (DO.Enums.CallType)boCall.CallType, boCall.FullAddress, boCall.OpenTime, false, boCall.Description, boCall.Latitude, boCall.Longitude, boCall.MaxCompletionTime);

        try
        {
            // Attempt to create the call in the data layer
            _dal.Call.Create(doCall);
        }
        catch (DO.DalAlreadyExistsException ex)
        {
            // Handle the case where the call already exists
            throw new BO.BlDoesNotExistException($"Call with ID={boCall.Id} already exists", ex);
        }
    }

    // Cancel an ongoing treatment (assignment)
    public void CancellationOfTreatment(int CancellerId, int assignmentId)
    {
        DO.Assignment? doAssignment = _dal.Assignment.Read(assignmentId);
        DO.Volunteer doVolunteer = _dal.Volunteer.Read(CancellerId)!;
        try
        {
            // Check if assignment exists
            if (doAssignment == null)
                throw new BO.BlDoesNotExistException($"Assignment with ID={assignmentId} does not exist.");

            // Validate cancellation conditions
            if (doAssignment.VolunteerId != CancellerId && doVolunteer.Role != DO.Enums.Role.Manager || doAssignment.CompletionTime != null || doAssignment.Status != null)
            {
                throw new BO.InvalidAssignmentCompletionException($"Assignment with ID={assignmentId} cannot be completed.");
            }
            else
            {
                // Update assignment status to canceled
                DO.Enums.TreatmentStatus finishType = doVolunteer.Role == DO.Enums.Role.Volunteer ? DO.Enums.TreatmentStatus.CanceledByVolunteer : DO.Enums.TreatmentStatus.CanceledByManager;
                DO.Assignment copyAssignment = doAssignment with { CompletionTime = ClockManager.Now, Status = finishType };
                _dal.Assignment.Update(copyAssignment);
            }
        }
        catch (BO.BlDoesNotExistException ex)
        {
            // Handle assignment not found
            throw;
        }
        catch (BO.InvalidAssignmentCompletionException ex)
        {
            // Handle invalid assignment state
            throw;
        }
    }

    // Delete a call from the system
    public void DeleteCall(int callId)
    {
        try
        {
            var doCall = _dal.Call.Read(callId)
                            ?? throw new BO.BlDoesNotExistException($"Call with ID={callId} does not exist.");

            // Check if the call is still open
            var statusCall = CallManager.GetStatusCall(callId);

            if (statusCall != BO.CallStatus.Open)
            {
                throw new BO.BlCannotBeDeletedException("Call must be in Open status to be deleted.");
            }

            // Check if the call is already assigned to a volunteer
            var assignment = _dal.Assignment.Read(a => a.CallId == callId);
            if (assignment != null)
            {
                throw new BO.BlCannotBeDeletedException("Call has been assigned to a volunteer and cannot be deleted.");
            }

            // Attempt to delete the call
            _dal.Call.Delete(callId);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            // Handle the case where the call does not exist
            throw new BO.BlDoesNotExistException($"Call with ID={callId} does not exist in the data layer.", ex);
        }
        catch (BO.BlCannotBeDeletedException)
        {
            // Handle the case where the call cannot be deleted
            throw;
        }
    }

    // Mark the call as finished (when an assignment is completed)
    public void FinishCall(int volunteerId, int assignmentId)
    {
        DO.Assignment? doAssignment = _dal.Assignment.Read(assignmentId);
        try
        {
            // Check if the assignment exists
            if (doAssignment == null)
                throw new BO.BlDoesNotExistException($"Assignment with ID={assignmentId} does not exist.");

            // Validate if the assignment can be completed
            if (doAssignment.VolunteerId != volunteerId || doAssignment.Status != null || doAssignment.CompletionTime != null)
            {
                throw new BO.InvalidAssignmentCompletionException($"Assignment with ID={assignmentId} cannot be completed.");
            }
            else
            {
                // Mark the assignment as completed on time
                DO.Assignment copyAssignment = doAssignment with { CompletionTime = ClockManager.Now, Status = DO.Enums.TreatmentStatus.CompletedOnTime };
                _dal.Assignment.Update(copyAssignment);
            }
        }
        catch (BO.BlDoesNotExistException ex)
        {
            // Handle assignment not found
            throw;
        }
        catch (BO.InvalidAssignmentCompletionException ex)
        {
            // Handle invalid completion
            throw;
        }
    }

    // Get the number of calls by status
    public int[] GetCallCounts()
    {
        var callList = _dal.Call.ReadAll();
        var statusValues = Enum.GetValues(typeof(BO.CallStatus)).Cast<BO.CallStatus>();
        int[] callCounts = new int[statusValues.Count()];

        var groupedCalls = callList
            .GroupBy(item => CallManager.GetStatusCall(item.Id))
            .ToDictionary(group => group.Key, group => group.Count());

        // Assign call counts to each status
        foreach (var status in statusValues)
        {
            callCounts[(int)status] = groupedCalls.GetValueOrDefault(status, 0);
        }

        return callCounts;
    }

    // Read the details of a specific call
    public BO.Call? Read(int callId)
    {
        var doCall = _dal.Call.Read(callId) ??
        throw new BO.BlDoesNotExistException($"Call with ID={callId} does Not exist");

        // Return call details in a BO.Call object
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

    // Get a list of calls with optional filtering and sorting
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

        // Apply filtering and sorting if necessary
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

    // Get a list of closed calls by a volunteer with optional filtering and sorting
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

    // Get a list of open calls for a volunteer with optional filtering and sorting
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
                DistanceFromVolunteer = CallManager.CalculateDistance(volunteerId, item.Latitude, item.Longitude)
            };
        })

        .ToList();

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

    // Assign a volunteer to a call
    public void SelectCall(int volunteerId, int callId)
    {
        DO.Call? doCall = _dal.Call.Read(callId);
        DO.Assignment? doAssignment = _dal.Assignment.Read(a => a.CallId == callId);
        try
        {
            // Check if the call is already assigned or in progress
            if (doAssignment != null || CallManager.GetStatusCall(callId) == BO.CallStatus.InProgress
                || CallManager.GetStatusCall(callId) == BO.CallStatus.InProgressAtRisk
                || doCall.MaxCompletionTime != null && doCall.MaxCompletionTime >= ClockManager.Now)
            {
                throw new BO.InvalidCallSelectionException($"Call with ID={callId} cannot be selected.");
            }
            else
            {
                // Create a new assignment for the volunteer
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
            // Handle invalid call selection
            throw;
        }
    }

    // Update the details of an existing call
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
            // Handle failed update
            throw new BO.BlCannotUpdateException($"Failed to update call with ID={doCall.Id}.", ex);
        }
        catch (BO.BlValidationException)
        {
            // Handle validation failure
            throw;
        }
        catch (BO.BlNullPropertyException)
        {
            // Handle missing property
            throw;
        }
    }
}
