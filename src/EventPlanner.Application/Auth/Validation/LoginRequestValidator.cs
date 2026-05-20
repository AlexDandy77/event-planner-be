using EventPlanner.Application.Auth.Dtos;
using EventPlanner.Application.Common.Validation;

namespace EventPlanner.Application.Auth.Validation;

public sealed class LoginRequestValidator : IRequestValidator<LoginRequest>
{
    public void Validate(LoginRequest request)
    {
        var errors = new ValidationErrorBag();
        var normalizedEmail = request.Email?.Trim();

        if (string.IsNullOrWhiteSpace(normalizedEmail))
        {
            errors.Add("email", "Email is required.");
        }
        else if (!AuthValidationRules.IsValidEmail(normalizedEmail))
        {
            errors.Add("email", "Email must be valid.");
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            errors.Add("password", "Password is required.");
        }

        errors.ThrowIfAny();
    }
}
