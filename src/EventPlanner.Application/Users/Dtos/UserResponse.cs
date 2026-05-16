namespace EventPlanner.Application.Users.Dtos;

public sealed record UserResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string Role,
    string? Phone,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt
);
