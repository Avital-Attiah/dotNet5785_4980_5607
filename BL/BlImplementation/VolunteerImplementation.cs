using BlApi;
using BO;
using Helpers;
using System.Collections.Generic;
using System.Data;
using static DO.Enums;
using System.Net;
using System;
using System.Linq;
namespace BlImplementation;

// Implementation of IVolunteer interface for managing Volunteer-related operations
internal class VolunteerImplementation : IVolunteer
{
    #region Stage 5
    public void AddObserver(Action listObserver) =>
    VolunteerManager.Observers.AddListObserver(listObserver); //stage 5
    public void AddObserver(int id, Action observer) =>
    VolunteerManager.Observers.AddObserver(id, observer); //stage 5
    public void RemoveObserver(Action listObserver) =>
    VolunteerManager.Observers.RemoveListObserver(listObserver); //stage 5
    public void RemoveObserver(int id, Action observer) =>
    VolunteerManager.Observers.RemoveObserver(id, observer); //stage 5
    #endregion Stage 5

    // Reference to the Data Access Layer (DAL) to interact with data storage
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    // Creates a new volunteer after validating the input
    public void Create(BO.Volunteer boVolunteer)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        VolunteerManager.ValidateVolunteer(boVolunteer); // כבר מחושב שם ה-Lat & Lon

        DO.Volunteer doVolunteer = new DO.Volunteer(
            boVolunteer.Id,
            boVolunteer.FullName,
            boVolunteer.Phone,
            boVolunteer.Email,
            VolunteerManager.HashPassword(boVolunteer.Password),
            boVolunteer.Address,
            boVolunteer.Latitude, 
            boVolunteer.Longitude, 
            (DO.Enums.Role)boVolunteer.Role,
            boVolunteer.IsActive,
            boVolunteer.MaxCallDistance,
            (DO.Enums.DistanceType)boVolunteer.DistanceType
        );

        try
        {
            lock (AdminManager.BlMutex)
                _dal.Volunteer.Create(doVolunteer);

            VolunteerManager.Observers.NotifyListUpdated();
        }
        catch (DO.DalAlreadyExistsException ex)
        {
            throw new BO.BlAlreadyExistsException($"Volunteer with ID={boVolunteer.Id} already exists.", ex);
        }
    }






    // Deletes an existing volunteer
    public void Delete(int id)
    {
        AdminManager.ThrowOnSimulatorIsRunning(); // ✅ זה יזרוק חריגה כשהסימולטור רץ

        try
        {
            BO.Volunteer boVolunteer = Read(id);

            if (boVolunteer.CurrentCall != null || boVolunteer.TotalCompletedCalls > 0)
            {
                throw new BO.BlCannotBeDeletedException($"Volunteer with ID={id} cannot be deleted because they are currently handling or have handled calls.");
            }

            _dal.Volunteer.Delete(id);
            VolunteerManager.Observers.NotifyListUpdated();
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


    // Fetches the role of a user based on their username and password
    public BO.Role GetUserRole(string userName, string password)
    {
        try
        {
            // Retrieve the user from the DAL based on their username
            var user = _dal.Volunteer.Read(v => v.FullName == userName) ?? throw new BO.BlDoesNotExistException($"Volunteer with UserName={userName} does not exist.");

            string hashedPassword = VolunteerManager.HashPassword(password);
            // Check if the password matches
            if (user.Password != hashedPassword)
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
        // שולף את המתנדב ממסד הנתונים
        var doVolunteer = _dal.Volunteer.Read(id)
            ?? throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does not exist");

        // שולף את כל השיבוצים של המתנדב
        var doAssignmentList = _dal.Assignment.ReadAll(a => a.VolunteerId == id).ToList();

        // מחשב סיכומים על הקריאות
        var (TotalCompletedCalls, totalCancelledCalls, totalExpiredSelectedCalls) =
            VolunteerManager.GetTotalsCalls(doAssignmentList);

        // שיבוץ אחרון של המתנדב
        var doAssignment = doAssignmentList.LastOrDefault();

        // אם יש שיבוץ ואין CompletionTime, נבנה CallInProgress
        BO.CallInProgress? callInProgress = null;
        if (doAssignment is { CompletionTime: null })
        {
            var doCall = _dal.Call.Read(doAssignment.CallId);
            if (doCall != null)
            {
                callInProgress = new BO.CallInProgress
                {
                    Id = doAssignment.Id,
                    CallId = doAssignment.CallId,
                    CallType = (BO.CallType)doCall.CallType,
                    Description = doCall.Description,
                    FullAddress = doCall.FullAddress,
                    OpeningTime = doCall.OpenTime,
                    StartHandlingTime = doAssignment.EntryTime,
                    DistanceFromVolunteer = CallManager.CalculateDistance(id, doCall.Latitude ?? 0, doCall.Longitude ?? 0),
                    Status = CallManager.GetStatusCall(doAssignment.CallId) switch
                    {
                        BO.CallStatus.InProgress or BO.CallStatus.InProgressAtRisk => BO.CallProgress.InTreatment,
                        _ => BO.CallProgress.AtRisk
                    }
                };
            }
        }

        // מחזיר את האובייקט המלא
        return new BO.Volunteer
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
            CurrentCall = callInProgress
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
                VolunteerManager.Observers.NotifyItemUpdated(doVolunteer.Id);  //stage 5
                VolunteerManager.Observers.NotifyListUpdated();  //stage 5

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
