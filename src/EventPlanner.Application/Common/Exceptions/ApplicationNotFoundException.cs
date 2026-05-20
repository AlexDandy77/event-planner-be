namespace EventPlanner.Application.Common.Exceptions;

public sealed class ApplicationNotFoundException(string message) : Exception(message)
{
}
