using DO;

namespace BO;

// Exception thrown when an entity already exists
[Serializable]
public class BlAlreadyExistsException : Exception
{
    public BlAlreadyExistsException(string? message) : base(message) { }
    public BlAlreadyExistsException(string message, Exception innerException)
                : base(message, innerException) { }
}

// Exception thrown when an entity does not exist
public class BlDoesNotExistException : Exception
{
    public BlDoesNotExistException(string? message) : base(message) { }
    public BlDoesNotExistException(string message, Exception innerException)
                : base(message, innerException) { }
}

// Exception thrown when an update operation cannot be performed
public class BlCannotUpdateException : Exception
{
    public BlCannotUpdateException(string? message) : base(message) { }
    public BlCannotUpdateException(string message, Exception innerException)
                : base(message, innerException) { }
}

// Exception thrown when an entity cannot be deleted
public class BlCannotBeDeletedException : Exception
{
    public BlCannotBeDeletedException(string? message) : base(message) { }
    public BlCannotBeDeletedException(string message, Exception innerException)
                : base(message, innerException) { }
}

// Exception thrown when a required property is null
public class BlNullPropertyException : Exception
{
    public BlNullPropertyException(string? message) : base(message) { }
}

// Exception thrown when there is an unauthorized access attempt
public class BlUnauthorizedAccessException : Exception
{
    public BlUnauthorizedAccessException(string? message) : base(message) { }
}

// Exception thrown when there is a validation error
public class BlValidationException : Exception
{
    public BlValidationException(string message) : base(message) { }
}

// Exception thrown when a date is invalid
public class BlInvalidDate : Exception
{
    public BlInvalidDate(string message) : base(message) { }
}

// Exception thrown when an assignment completion is invalid
public class InvalidAssignmentCompletionException : Exception
{
    public InvalidAssignmentCompletionException(string message) : base(message) { }
}

// Exception thrown when there is an invalid call selection
public class InvalidCallSelectionException : Exception
{
    public InvalidCallSelectionException(string message) : base(message) { }
}
