namespace EventPlanner.Application.Common.Exceptions;

public sealed class ApplicationConflictException(string message) : Exception(message)
{
}
