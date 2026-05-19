namespace EventPlanner.Application.Common.Exceptions;

public sealed class ApplicationForbiddenException(string message) : Exception(message)
{
}
