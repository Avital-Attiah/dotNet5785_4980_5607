namespace BO
{
 
    // Enum representing the status of the call.
    public enum CallInProgressStatus
    {
        InProgress,
        AtRisk
    }

    /// סוגי הקריאות השונות.
    public enum CallType
    {
        EmotionalSupport,
        FamilySupport,
        ProfessionalConsultation,
        Emergency,
        None
    }

    // Enum representing the role of a volunteer within the system.
    public enum Role
    {
        Manager,
        Volunteer
    }

    // Enum representing the type of distance calculation.
    public enum DistanceType
    {
        Air,
        Walking,
        Car
    }

    // Enum representing the status of an assignment.
    public enum TreatmentStatus
    {
        CompletedOnTime,
        CanceledByVolunteer,
        CanceledByManager,
        Expired
    }

    /// Represents the types of closure for a call.
    public enum ClosureType
    {
        Handled,
        Canceled,
        Expired
    }
    /// Represents the type of treatment completion for a call assignment.
    public enum CompletionType
    {
        Completed,// Treatment was successfully completed.
        Canceled,// Call was canceled.
        Expired, // Call was expired without being handled.
        None// No treatment completion type (default).
    }
    /// Represents the status of a call in the system.
    public enum CallStatus
    {
        Open,// Call is currently open and not assigned to any volunteer.
        InProgress,// Call is currently being handled by a volunteer.
        Closed,// Call was successfully completed and closed.
        Expired,// Call was not handled in time and is now expired.
        OpenAtRisk,// Call is open and nearing its maximum allowed time to be resolved.
        InProgressAtRisk// Call is in progress and nearing its maximum allowed time to be resolved.
    }

    public enum CallProgress
    {
        InTreatment,
        AtRisk
    }

    //שיטת מיון למתנדב
    public enum VolunteerInLIstFields
    {
        Id,
        FullName,
        IsActive,
        TotalHandledCalls,
        TotalCancelledCalls,
        TotalExpiredSelectedCalls,
        CallId,
        TypeCall
    }


    public enum CallInListFieldSor
    {
        Id,
        CallId,
        CallType,
        TimeOpen,
        RemainingTime,
        LastVolunteerName,
        CompletionTime,
        Status,
        ListAssignments
    }
    public enum ClosedCallInListFields
    {
        Id,
        TypeCall,
        AddressOfCall,
        OpenTime,
        EntryTimeToHandling,
        ActualEndTime,
        FinishType
    }

    public enum OpenCallInListFields
    {
        Id,
        typeCall,
        VerbalDescription,
        FullAddressOfCall,
        TimeOpen,
        MaxTimeFinishCall,
        VolunteerDistance
    }



}

