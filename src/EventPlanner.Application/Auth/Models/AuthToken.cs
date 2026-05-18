namespace EventPlanner.Application.Auth.Models;

public sealed record AuthToken(string AccessToken, int ExpiresIn);
