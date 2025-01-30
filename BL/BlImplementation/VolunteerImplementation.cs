using BlApi;
using BO;
using Helpers;
using System.Collections.Generic;
using System.Data;
using static DO.Enums;
using System.Net;


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
                boVolunteer.Id, boVolunteer.Name, boVolunteer.PhoneNumber,
                boVolunteer.Email, boVolunteer.Password, boVolunteer.Address,
                boVolunteer.Latitude, boVolunteer.Longitude, (DO.RoleT)boVolunteer.Role,
                boVolunteer.Active, boVolunteer.Distance, (DO.TypeDistance)boVolunteer.DistanceType));
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

            if (boVolunteer.VolunteerCallInProgress != null || boVolunteer.TotalHandledCalls > 0)
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
            var user = _dal.Volunteer.Read(v => v.Name == userName) ?? throw new BO.BlDoesNotExistException($"Volunteer with UserName={userName} does not exist.");

            if (user.Password != password)
            {
                throw new BO.BlValidationException("Incorrect password");
            }

            return (BO.RoleT)user.Role;
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
        : _dal.Volunteer.ReadAll(v => v.Active == filterByActive);

        var volunteersInList = volunteers.Select(item => {
            var doAssignmentList = _dal.Assignment.ReadAll(a => a.VolunteerId == item.Id);
            var (totalHandledCalls, totalCancelledCalls, totalExpiredSelectedCalls) = VolunteerManager.GetTotalsCalls(doAssignmentList);
            var doAssignment = _dal.Assignment.Read(a => a.TypeFinish == null);

            return new BO.VolunteerInList
            {
                Id = item.Id,
                FullName = item.Name,
                IsActive = item.Active,
                TotalHandledCalls = totalHandledCalls,
                TotalCancelledCalls = totalCancelledCalls,
                TotalExpiredSelectedCalls = totalExpiredSelectedCalls,
                CallId = doAssignment != null ? doAssignment.CallId : null,
                TypeCall = doAssignment == null ? BO.TypeCall.None
                : (BO.TypeCall)_dal.Call.Read(doAssignment.CallId)!.CallType
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

        #region  //if (sortByField != null)
        //{
        //    volunteersInList = sortByField switch
        //    {
        //        BO.VolunteerInLIstFields.Id => volunteersInList
        //            .OrderByDescending(v => v.Id).ToList(),
        //        BO.VolunteerInLIstFields.FullName => volunteersInList
        //            .OrderByDescending(v => v.FullName).ToList(),
        //        BO.VolunteerInLIstFields.IsActive => volunteersInList
        //            .OrderByDescending(v => v.IsActive).ToList(),
        //        BO.VolunteerInLIstFields.TotalHandledCalls => volunteersInList
        //       .OrderByDescending(v => v.TotalHandledCalls).ToList(),
        //        BO.VolunteerInLIstFields.TotalCancelledCalls => volunteersInList
        //            .OrderByDescending(v => v.TotalCancelledCalls).ToList(),
        //        BO.VolunteerInLIstFields.TotalExpiredSelectedCalls => volunteersInList
        //       .OrderByDescending(v => v.TotalExpiredSelectedCalls).ToList(),
        //        BO.VolunteerInLIstFields.CallId => volunteersInList
        //       .OrderByDescending(v => v.CallId).ToList(),
        //        BO.VolunteerInLIstFields.TypeCall => volunteersInList
        //       .OrderByDescending(v => v.TypeCall).ToList(),
        //        _ => volunteersInList
        //    };

        //}
        //else
        //{
        //    volunteersInList.OrderByDescending(v => v.Id).ToList();
        //} 
        #endregion
    }

    public BO.Volunteer Read(int id)
    {
        var doVolunteer = _dal.Volunteer.Read(id) ??
        throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does Not exist");
        var doAssignmentList = _dal.Assignment.ReadAll(a => a.VolunteerId == id);
        var (totalHandledCalls, totalCancelledCalls, totalExpiredSelectedCalls) = VolunteerManager.GetTotalsCalls(doAssignmentList);
        var doAssignment = doAssignmentList.LastOrDefault();
        BO.CallInProgress? callInProgress;
        if (doAssignment != null && doAssignment.TypeFinish == null)
        {
            var doCall = _dal.Call.Read(doAssignment.CallId);
            callInProgress = new BO.CallInProgress()
            {
                Id = doAssignment.Id,
                CallId = doAssignment.CallId,
                TypeCall = (BO.TypeCall)doCall.CallType,
                Description = doCall.VerbalDescription,
                AddressOfCall = doCall.FullAddressOfCall,
                OpenTime = doCall.TimeOpen,
                EntryTimeToHandling = doAssignment.TimeEnter,
                DistanceFromVolunteer = CallManager.GetDistanceBetweenAddresses(doVolunteer.Address, doCall.FullAddressOfCall),
                Status = CallManager.GetStatusCall(doAssignment.CallId) == BO.StatusCall.InProgress ? BO.StatusCallProgress.InHandling : BO.StatusCallProgress.InRiskHandling
            };
        }
        else
        {
            callInProgress = null;
        }
        return new()
        {
            Id = id,
            Name = doVolunteer.Name,
            PhoneNumber = doVolunteer.PhoneNumber,
            Email = doVolunteer.Email,
            Password = doVolunteer.Password,
            Address = doVolunteer.Address,
            Latitude = doVolunteer.Latitude,
            Longitude = doVolunteer.Longitude,
            Role = (BO.RoleT)doVolunteer.Role,
            Active = doVolunteer.Active,
            Distance = doVolunteer.Distance,
            DistanceType = (BO.TypeDistance)doVolunteer.DistanceType,
            TotalHandledCalls = totalHandledCalls,
            TotalCancelledCalls = totalCancelledCalls,
            TotalExpiredSelectedCalls = totalExpiredSelectedCalls,
            VolunteerCallInProgress = callInProgress
        };
    }


    public void Update(int id, BO.Volunteer updateVolunteerObj)
    {
        if (!VolunteerManager.IsValidId(id))
        {
            throw new BO.BlValidationException("Invalid ID number.");
        }

        var doVolunteer = _dal.Volunteer.Read(id)
                          ?? throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does not exist");

        if (doVolunteer.Role == DO.RoleT.manager || doVolunteer.Id == id)
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