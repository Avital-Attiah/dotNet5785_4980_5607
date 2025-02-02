using BlApi;
using BO;
using Helpers;
using System.Collections.Generic;
using System.Data;
using static DO.Enums;
using System.Net;
using System;



namespace BlImplementation;
internal class VolunteerImplementation : IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    public void Create(BO.Volunteer boVolunteer)
    {
        // Perform validation
        VolunteerManager.ValidateVolunteer(boVolunteer);

        try
        {
            _dal.Volunteer.Create(new DO.Volunteer(
                boVolunteer.Id, boVolunteer.FullName, boVolunteer.Phone,
                boVolunteer.Email, boVolunteer.Password, boVolunteer.Address,
                boVolunteer.Latitude, boVolunteer.Longitude, (DO.Enums.Role)boVolunteer.Role,
                boVolunteer.IsActive, boVolunteer.MaxCallDistance, (DO.Enums.DistanceType)boVolunteer.DistanceType));
        }
        catch (DO.DalAlreadyExistsException ex)
        {
            throw new BO.BlAlreadyExistsException($"Volunteer with ID={boVolunteer.Id} already exists.", ex);
        }
    }


    public void Delete(int id)
    {
        try
        {
            BO.Volunteer boVolunteer = Read(id);

            if (boVolunteer.CurrentCall != null || boVolunteer.TotalCompletedCalls > 0)
            {
                throw new BO.BlCannotBeDeletedException($"Volunteer with ID={id} cannot be deleted because they are currently handling or have handled calls.");
            }

            _dal.Volunteer.Delete(id);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does not exist.", ex);
        }
        catch (BO.BlCannotBeDeletedException)
        {
            throw new BO.BlCannotBeDeletedException($"Failed to delete volunteer with ID={id}.");
        }
    }

    public BO.Role GetUserRole(string userName, string password)
    {
        try
        {
            var user = _dal.Volunteer.Read(v => v.FullName == userName) ?? throw new BO.BlDoesNotExistException($"Volunteer with UserName={userName} does not exist.");

            if (user.Password != password)
            {
                throw new BO.BlValidationException("Incorrect password");
            }

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



    public IEnumerable<BO.VolunteerInList> GetVolunteersList(bool? filterByActive = null, BO.VolunteerInLIstFields? sortByField = null)
    {
        var volunteers = filterByActive == null
        ? _dal.Volunteer.ReadAll()
        : _dal.Volunteer.ReadAll(v => v.IsActive == filterByActive);

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

    public BO.Role Login(string fullName, string password)
    {

        var volunteer = _dal.Volunteer.Read(v => v.FullName == fullName && v.Password == password);
        if (volunteer == null)
        {
            throw new BO.BlDoesNotExistException("Incorrect username or password");
        }
        return (BO.Role)volunteer.Role;
    }

    public void Update(int id, BO.Volunteer updateVolunteerObj)
    {
        if (!VolunteerManager.IsValidId(id))
        {
            throw new BO.BlValidationException("Invalid ID number.");
        }

        var doVolunteer = _dal.Volunteer.Read(id)
                          ?? throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does not exist");

        if (doVolunteer.Role == DO.Enums.Role.Manager || doVolunteer.Id == id)
        {
            try
            {
                // Perform validation
                VolunteerManager.ValidateVolunteer(updateVolunteerObj, isUpdate: true);
                var copyDoVolunteer = VolunteerManager.updateDoVolunteerIfNeeded(doVolunteer, updateVolunteerObj);
                _dal.Volunteer.Update(copyDoVolunteer);
            }
            catch (DO.DalDoesNotExistException ex)
            {
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
            throw new BO.BlUnauthorizedAccessException("You do not have permission to update this volunteer's details.");
        }
    }

}


