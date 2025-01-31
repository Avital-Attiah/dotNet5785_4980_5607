using DO;

namespace BO;
[Serializable]

public class BlAlreadyExistsException : Exception
{
    public BlAlreadyExistsException(string? message) : base(message) { }
    public BlAlreadyExistsException(string message, Exception innerException)
                : base(message, innerException) { }
}

public class BlDoesNotExistException : Exception
{
    public BlDoesNotExistException(string? message) : base(message) { }
    public BlDoesNotExistException(string message, Exception innerException)
                : base(message, innerException) { }
}
public class BlCannotUpdateException : Exception
{
    public BlCannotUpdateException(string? message) : base(message) { }
    public BlCannotUpdateException(string message, Exception innerException)
                : base(message, innerException) { }
}
public class BlCannotBeDeletedException : Exception
{
    public BlCannotBeDeletedException(string? message) : base(message) { }
    public BlCannotBeDeletedException(string message, Exception innerException)
                : base(message, innerException) { }
}
public class BlNullPropertyException : Exception
{
    public BlNullPropertyException(string? message) : base(message) { }
}
public class BlUnauthorizedAccessException : Exception
{
    public BlUnauthorizedAccessException(string? message) : base(message) { }
}

public class BlValidationException : Exception
{
    public BlValidationException(string message) : base(message) { }
}
public class BlInvalidDate : Exception
{
    public BlInvalidDate(string message) : base(message) { }
}

public class InvalidAssignmentCompletionException : Exception
{
    public InvalidAssignmentCompletionException(string message) : base(message) { }
}
public class InvalidCallSelectionException : Exception
{
    public InvalidCallSelectionException(string message) : base(message) { }
}



