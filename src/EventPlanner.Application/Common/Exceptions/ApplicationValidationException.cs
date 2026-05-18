namespace EventPlanner.Application.Common.Exceptions;

public sealed class ApplicationValidationException(string message) : Exception(message)
{
}
