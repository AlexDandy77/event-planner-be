using EventPlanner.Application.Common.Exceptions;

namespace EventPlanner.Application.Common.Validation;

public sealed class ValidationErrorBag
{
    private readonly Dictionary<string, List<string>> _errors = new(StringComparer.Ordinal);

    public bool HasErrors => _errors.Count > 0;

    public void Add(string field, string message)
    {
        if (!_errors.TryGetValue(field, out var messages))
        {
            messages = [];
            _errors[field] = messages;
        }

        messages.Add(message);
    }

    public void ThrowIfAny()
    {
        if (!HasErrors)
        {
            return;
        }

        throw new ApplicationValidationException(
            _errors.ToDictionary(
                error => error.Key,
                error => error.Value.ToArray(),
                StringComparer.Ordinal
            )
        );
    }
}
