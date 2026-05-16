using EventPlanner.Application.Users.Dtos;

namespace EventPlanner.Application.Auth.Dtos;

public sealed record AuthResponse(
    string AccessToken,
    string TokenType,
    int ExpiresIn,
    UserResponse User
);
