namespace EventPlanner.Application.Common.Exceptions;

public sealed class ApplicationAuthenticationException(string message) : Exception(message)
{
}
