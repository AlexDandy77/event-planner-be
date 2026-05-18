using EventPlanner.Application.Auth.Dtos;
using EventPlanner.Application.Users.Dtos;

namespace EventPlanner.Application.Auth.Services;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken
    );

    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken);

    Task<UserResponse> GetCurrentUserAsync(CancellationToken cancellationToken);
}
