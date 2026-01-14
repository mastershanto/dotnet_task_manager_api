namespace TodoApi.Domain.Exceptions;

/// <summary>
/// Base domain exception for business rule violations
/// </summary>
public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message)
    {
    }

    protected DomainException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

public class TaskNotFoundException : DomainException
{
    public TaskNotFoundException(int taskId)
        : base($"Task with ID {taskId} was not found")
    {
    }
}

public class TaskAccessDeniedException : DomainException
{
    public TaskAccessDeniedException()
        : base("You don't have permission to access this task")
    {
    }
}

public class InvalidTaskStatusTransitionException : DomainException
{
    public InvalidTaskStatusTransitionException(string currentStatus, string newStatus)
        : base($"Cannot transition task from {currentStatus} to {newStatus}")
    {
    }
}

public class ProjectNotFoundException : DomainException
{
    public ProjectNotFoundException(int projectId)
        : base($"Project with ID {projectId} was not found")
    {
    }
}
