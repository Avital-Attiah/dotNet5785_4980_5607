using BlApi;
using BO;
using Helpers;
using System.Collections.Generic;
using System.Data;
using static DO.Enums;
using System.Net;
using System;

namespace BlImplementation;

// Implementation of IVolunteer interface for managing Volunteer-related operations
internal class VolunteerImplementation : IVolunteer
{
    // Reference to the Data Access Layer (DAL) to interact with data storage
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    // Creates a new volunteer after validating the input
    public void Create(BO.Volunteer boVolunteer)
    {
        // Validate the volunteer object
        VolunteerManager.ValidateVolunteer(boVolunteer);

        try
        {
            // Create a new volunteer in the DAL
            _dal.Volunteer.Create(new DO.Volunteer(
                boVolunteer.Id, boVolunteer.FullName, boVolunteer.Phone,
                boVolunteer.Email, boVolunteer.Password, boVolunteer.Address,
                boVolunteer.Latitude, boVolunteer.Longitude, (DO.Enums.Role)boVolunteer.Role,
                boVolunteer.IsActive, boVolunteer.MaxCallDistance, (DO.Enums.DistanceType)boVolunteer.DistanceType));
        }
        catch (DO.DalAlreadyExistsException ex)
        {
            // Handle the case where the volunteer already exists
            throw new BO.BlAlreadyExistsException($"Volunteer with ID={boVolunteer.Id} already exists.", ex);
        }
    }

    // Deletes an existing volunteer
    public void Delete(int id)
    {
        try
        {
            // Read the volunteer details by ID
            BO.Volunteer boVolunteer = Read(id);

            // Check if the volunteer is currently handling calls or has handled any calls
            if (boVolunteer.CurrentCall != null || boVolunteer.TotalCompletedCalls > 0)
            {
                throw new BO.BlCannotBeDeletedException($"Volunteer with ID={id} cannot be deleted because they are currently handling or have handled calls.");
            }

            // Delete the volunteer from the DAL
            _dal.Volunteer.Delete(id);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            // Handle case where the volunteer does not exist
            throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does not exist.", ex);
        }
        catch (BO.BlCannotBeDeletedException)
        {
            throw new BO.BlCannotBeDeletedException($"Failed to delete volunteer with ID={id}.");
        }
    }

    // Fetches the role of a user based on their username and password
    public BO.Role GetUserRole(string userName, string password)
    {
        try
        {
            // Retrieve the user from the DAL based on their username
            var user = _dal.Volunteer.Read(v => v.FullName == userName) ?? throw new BO.BlDoesNotExistException($"Volunteer with UserName={userName} does not exist.");

            // Check if the password matches
            if (user.Password != password)
            {
                throw new BO.BlValidationException("Incorrect password");
            }

            // Return the role of the user
            return (BO.Role)user.Role;
        }
        catch (BO.BlDoesNotExistException)
        {
            throw;
        }
        catch (BO.BlValidationException)
        {
            throw;
        }
    }

    // Retrieves a list of volunteers with optional filtering and sorting
    public IEnumerable<BO.VolunteerInList> GetVolunteersList(bool? filterByActive = null, BO.VolunteerInLIstFields? sortByField = null)
    {
        // Read all volunteers, optionally filter by active status
        var volunteers = filterByActive == null
        ? _dal.Volunteer.ReadAll()
        : _dal.Volunteer.ReadAll(v => v.IsActive == filterByActive);

        // Transform the volunteer data to a list of BO.VolunteerInList objects
        var volunteersInList = volunteers.Select(item => {
            var doAssignmentList = _dal.Assignment.ReadAll(a => a.VolunteerId == item.Id);
            var (TotalCompletedCalls, TotalCanceledCalls, TotalExpiredCalls) = VolunteerManager.GetTotalsCalls(doAssignmentList);
            var doAssignment = _dal.Assignment.Read(a => a.CompletionTime == null);

            return new BO.VolunteerInList
            {
                Id = item.Id,
                FullName = item.FullName,
                IsActive = item.IsActive,
                TotalHandledCalls = TotalCompletedCalls,
                TotalCanceledCalls = TotalCanceledCalls,
                TotalExpiredCalls = TotalExpiredCalls,
                CurrentCallId = doAssignment != null ? doAssignment.CallId : null,
                CurrentCallType = doAssignment == null ? BO.CallType.None
                : (BO.CallType)_dal.Call.Read(doAssignment.CallId)!.CallType
            };
        })
          .ToList();

        // Sort the list based on the provided sorting field
        if (sortByField != null)
        {
            volunteersInList = Tools.SortByEnum<BO.VolunteerInList, BO.VolunteerInLIstFields>(volunteersInList, sortByField);
        }
        else
        {
            volunteersInList = volunteersInList.OrderByDescending(v => v.Id).ToList();
        }

        return volunteersInList;
    }

    // Retrieves detailed information about a specific volunteer
    public BO.Volunteer Read(int id)
    {
        var doVolunteer = _dal.Volunteer.Read(id) ??
        throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does Not exist");
        var doAssignmentList = _dal.Assignment.ReadAll(a => a.VolunteerId == id);
        var (TotalCompletedCalls, totalCancelledCalls, totalExpiredSelectedCalls) = VolunteerManager.GetTotalsCalls(doAssignmentList);
        var doAssignment = doAssignmentList.LastOrDefault();
        BO.CallInProgress? callInProgress;

        if (doAssignment != null && doAssignment.CompletionTime == null)
        {
            var doCall = _dal.Call.Read(doAssignment.CallId);
            callInProgress = new BO.CallInProgress()
            {
                Id = doAssignment.Id,
                CallId = doAssignment.CallId,
                CallType = (BO.CallType)doCall.CallType,
                Description = doCall.Description,
                FullAddress = doCall.FullAddress,
                OpeningTime = doCall.OpenTime,
                StartHandlingTime = doAssignment.EntryTime,
                DistanceFromVolunteer = CallManager.CalculateDistance(id, doCall.Latitude, doCall.Longitude),
                Status = CallManager.GetStatusCall(doAssignment.CallId) == BO.CallStatus.InProgress ? BO.CallProgress.InTreatment : BO.CallProgress.AtRisk
            };
        }
        else
        {
            callInProgress = null;
        }

        // Return the detailed volunteer information
        return new()
        {
            Id = id,
            FullName = doVolunteer.FullName,
            Phone = doVolunteer.Phone,
            Email = doVolunteer.Email,
            Password = doVolunteer.Password,
            Address = doVolunteer.CurrentAddress,
            Latitude = doVolunteer.Latitude,
            Longitude = doVolunteer.Longitude,
            Role = (BO.Role)doVolunteer.Role,
            IsActive = doVolunteer.IsActive,
            MaxCallDistance = doVolunteer.MaxDistance,
            DistanceType = (BO.DistanceType)doVolunteer.DistanceType,
            TotalCompletedCalls = TotalCompletedCalls,
            TotalCanceledCalls = totalCancelledCalls,
            TotalExpiredCalls = totalExpiredSelectedCalls,
            CurrentCall = callInProgress?.Status
        };
    }

    // Logs in a volunteer by validating their credentials
    public BO.Role Login(string fullName, string password)
    {
        var volunteer = _dal.Volunteer.Read(v => v.FullName == fullName && v.Password == password);
        if (volunteer == null)
        {
            throw new BO.BlDoesNotExistException("Incorrect username or password");
        }
        return (BO.Role)volunteer.Role;
    }

    // Updates an existing volunteer's details
    public void Update(int id, BO.Volunteer updateVolunteerObj)
    {
        // Validate the ID
        if (!VolunteerManager.IsValidId(id))
        {
            throw new BO.BlValidationException("Invalid ID number.");
        }

        var doVolunteer = _dal.Volunteer.Read(id)
                          ?? throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does not exist");

        // Check if the user has permission to update the volunteer details
        if (doVolunteer.Role == DO.Enums.Role.Manager || doVolunteer.Id == id)
        {
            try
            {
                // Validate the updated volunteer data
                VolunteerManager.ValidateVolunteer(updateVolunteerObj, isUpdate: true);
                var copyDoVolunteer = VolunteerManager.updateDoVolunteerIfNeeded(doVolunteer, updateVolunteerObj);
                _dal.Volunteer.Update(copyDoVolunteer);
            }
            catch (DO.DalDoesNotExistException ex)
            {
                // Handle the case where the volunteer cannot be updated
                throw new BO.BlCannotUpdateException($"Failed to update volunteer with ID={id}.", ex);
            }
            catch (BO.BlValidationException)
            {
                throw;
            }
            catch (BO.BlNullPropertyException)
            {
                throw;
            }
            catch (BO.BlUnauthorizedAccessException)
            {
                throw;
            }
        }
        else
        {
            // Unauthorized access: The user cannot update this volunteer's details
            throw new BO.BlUnauthorizedAccessException("You do not have permission to update this volunteer's details.");
        }
    }
}
