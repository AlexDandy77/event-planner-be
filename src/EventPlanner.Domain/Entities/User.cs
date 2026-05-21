using EventPlanner.Domain.Common;
using EventPlanner.Domain.Enums;

namespace EventPlanner.Domain.Entities;

public sealed class User
{
    public const int MaxFirstNameLength = 80;
    public const int MaxLastNameLength = 80;
    public const int MaxEmailLength = 254;
    public const int MaxPhoneLength = 30;

    private User()
    {
    }

    private User(
        Guid id,
        string firstName,
        string lastName,
        string email,
        UserRole role,
        string? phone,
        string passwordHash,
        DateTimeOffset createdAt
    )
    {
        Id = id;
        Role = role;
        PasswordHash = NormalizePasswordHash(passwordHash);
        IsActive = true;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
        ApplyProfile(firstName, lastName, email, phone);
    }

    public Guid Id { get; private set; }

    public string FirstName { get; private set; } = string.Empty;

    public string LastName { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;

    public UserRole Role { get; private set; }

    public string? Phone { get; private set; }

    public string PasswordHash { get; private set; } = string.Empty;

    public bool IsActive { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public static User Create(
        string firstName,
        string lastName,
        string email,
        UserRole role,
        string? phone,
        string passwordHash,
        DateTimeOffset createdAt
    )
    {
        return new User(
            Guid.NewGuid(),
            firstName,
            lastName,
            email,
            role,
            phone,
            passwordHash,
            createdAt
        );
    }

    public void UpdateProfile(
        string firstName,
        string lastName,
        string email,
        string? phone,
        DateTimeOffset updatedAt
    )
    {
        ApplyProfile(firstName, lastName, email, phone);
        UpdatedAt = updatedAt;
    }

    public void ChangePasswordHash(string passwordHash, DateTimeOffset updatedAt)
    {
        PasswordHash = NormalizePasswordHash(passwordHash);
        UpdatedAt = updatedAt;
    }

    public void Activate(DateTimeOffset updatedAt)
    {
        IsActive = true;
        UpdatedAt = updatedAt;
    }

    public void Deactivate(DateTimeOffset updatedAt)
    {
        IsActive = false;
        UpdatedAt = updatedAt;
    }

    private void ApplyProfile(string firstName, string lastName, string email, string? phone)
    {
        FirstName = NormalizeRequired(firstName, nameof(FirstName), MaxFirstNameLength);
        LastName = NormalizeRequired(lastName, nameof(LastName), MaxLastNameLength);
        Email = NormalizeEmail(email);
        Phone = NormalizePhone(phone);
    }

    private static string NormalizeRequired(string value, string fieldName, int maxLength)
    {
        var normalizedValue = value.Trim();

        if (string.IsNullOrWhiteSpace(normalizedValue))
        {
            throw new DomainException($"{fieldName} is required.");
        }

        if (normalizedValue.Length > maxLength)
        {
            throw new DomainException($"{fieldName} cannot exceed {maxLength} characters.");
        }

        return normalizedValue;
    }

    private static string NormalizeEmail(string email)
    {
        var normalizedEmail = NormalizeRequired(email, nameof(Email), MaxEmailLength).ToLowerInvariant();

        if (!normalizedEmail.Contains('@', StringComparison.Ordinal))
        {
            throw new DomainException("Email must be valid.");
        }

        return normalizedEmail;
    }

    private static string? NormalizePhone(string? phone)
    {
        var normalizedPhone = phone?.Trim();

        if (string.IsNullOrEmpty(normalizedPhone))
        {
            return null;
        }

        if (normalizedPhone.Length > MaxPhoneLength)
        {
            throw new DomainException($"Phone cannot exceed {MaxPhoneLength} characters.");
        }

        return normalizedPhone;
    }

    private static string NormalizePasswordHash(string passwordHash)
    {
        var normalizedPasswordHash = passwordHash.Trim();

        if (string.IsNullOrWhiteSpace(normalizedPasswordHash))
        {
            throw new DomainException("Password hash is required.");
        }

        return normalizedPasswordHash;
    }
}
