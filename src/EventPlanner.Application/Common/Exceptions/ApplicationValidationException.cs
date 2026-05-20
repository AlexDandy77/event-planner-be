using System.Collections.ObjectModel;

namespace EventPlanner.Application.Common.Exceptions;

public sealed class ApplicationValidationException : Exception
{
    public ApplicationValidationException(string message)
        : base(message)
    {
        Errors = new ReadOnlyDictionary<string, string[]>(
            new Dictionary<string, string[]>
            {
                ["general"] = [message]
            }
        );
    }

    public ApplicationValidationException(IDictionary<string, string[]> errors)
        : base("One or more validation errors occurred.")
    {
        Errors = new ReadOnlyDictionary<string, string[]>(
            errors.ToDictionary(
                error => error.Key,
                error => error.Value,
                StringComparer.Ordinal
            )
        );
    }

    public IReadOnlyDictionary<string, string[]> Errors { get; }
}
