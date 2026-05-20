using EventPlanner.Application.Auth.Dtos;
using EventPlanner.Application.Common.Validation;
using EventPlanner.Domain.Entities;

namespace EventPlanner.Application.Auth.Validation;

public sealed class RegisterRequestValidator : IRequestValidator<RegisterRequest>
{
    private const int MinimumPasswordLength = 8;

    public void Validate(RegisterRequest request)
    {
        var errors = new ValidationErrorBag();

        ValidateRequiredText(
            request.FirstName,
            "firstName",
            "First name",
            User.MaxFirstNameLength,
            errors
        );
        ValidateRequiredText(
            request.LastName,
            "lastName",
            "Last name",
            User.MaxLastNameLength,
            errors
        );
        ValidateEmail(request.Email, errors);
        ValidatePassword(request.Password, errors);
        ValidatePhone(request.Phone, errors);

        errors.ThrowIfAny();
    }

    private static void ValidateRequiredText(
        string? value,
        string field,
        string displayName,
        int maxLength,
        ValidationErrorBag errors
    )
    {
        var normalizedValue = value?.Trim();

        if (string.IsNullOrWhiteSpace(normalizedValue))
        {
            errors.Add(field, $"{displayName} is required.");

            return;
        }

        if (normalizedValue.Length > maxLength)
        {
            errors.Add(field, $"{displayName} cannot exceed {maxLength} characters.");
        }
    }

    private static void ValidateEmail(string? email, ValidationErrorBag errors)
    {
        var normalizedEmail = email?.Trim();

        if (string.IsNullOrWhiteSpace(normalizedEmail))
        {
            errors.Add("email", "Email is required.");

            return;
        }

        if (normalizedEmail.Length > User.MaxEmailLength)
        {
            errors.Add("email", $"Email cannot exceed {User.MaxEmailLength} characters.");
        }

        if (!AuthValidationRules.IsValidEmail(normalizedEmail))
        {
            errors.Add("email", "Email must be valid.");
        }
    }

    private static void ValidatePassword(string? password, ValidationErrorBag errors)
    {
        var normalizedPassword = password?.Trim();

        if (string.IsNullOrWhiteSpace(normalizedPassword))
        {
            errors.Add("password", "Password is required.");

            return;
        }

        if (normalizedPassword.Length < MinimumPasswordLength)
        {
            errors.Add(
                "password",
                $"Password must be at least {MinimumPasswordLength} characters."
            );
        }
    }

    private static void ValidatePhone(string? phone, ValidationErrorBag errors)
    {
        var normalizedPhone = phone?.Trim();

        if (string.IsNullOrEmpty(normalizedPhone))
        {
            return;
        }

        if (normalizedPhone.Length > User.MaxPhoneLength)
        {
            errors.Add("phone", $"Phone cannot exceed {User.MaxPhoneLength} characters.");
        }
    }
}
