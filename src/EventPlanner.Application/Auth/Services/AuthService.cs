using EventPlanner.Application.Auth.Abstractions;
using EventPlanner.Application.Auth.Dtos;
using EventPlanner.Application.Common.Abstractions;
using EventPlanner.Application.Common.Exceptions;
using EventPlanner.Application.Users.Dtos;
using EventPlanner.Application.Users.Mapping;
using EventPlanner.Application.Users.Repositories;
using EventPlanner.Domain.Entities;
using EventPlanner.Domain.Enums;

namespace EventPlanner.Application.Auth.Services;

public sealed class AuthService(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IAuthTokenService authTokenService,
    ICurrentUserService currentUserService,
    IClock clock
) : IAuthService
{
    private const int MinimumPasswordLength = 8;
    private const string TokenType = "Bearer";

    private readonly IAuthTokenService _authTokenService = authTokenService;
    private readonly IClock _clock = clock;
    private readonly ICurrentUserService _currentUserService = currentUserService;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<AuthResponse> RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken
    )
    {
        var email = RequireEmail(request.Email);
        var password = RequirePassword(request.Password);

        if (await _userRepository.ExistsByEmailAsync(email, cancellationToken))
        {
            throw new ApplicationConflictException("A user with this email already exists.");
        }

        var user = User.Create(
            RequireText(request.FirstName, "First name"),
            RequireText(request.LastName, "Last name"),
            email,
            UserRole.Organizer,
            request.Phone,
            _passwordHasher.Hash(password),
            _clock.UtcNow
        );

        await _userRepository.AddAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        return CreateAuthResponse(user);
    }

    public async Task<AuthResponse> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken
    )
    {
        var email = RequireEmail(request.Email);
        var password = RequireText(request.Password, "Password");
        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);

        if (
            user is null
            || !user.IsActive
            || !_passwordHasher.Verify(password, user.PasswordHash)
        )
        {
            throw new ApplicationAuthenticationException("Invalid email or password.");
        }

        return CreateAuthResponse(user);
    }

    public async Task<UserResponse> GetCurrentUserAsync(CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        if (!userId.HasValue)
        {
            throw new ApplicationAuthenticationException("Authentication is required.");
        }

        var user = await _userRepository.GetByIdAsync(userId.Value, cancellationToken);

        if (user is null || !user.IsActive)
        {
            throw new ApplicationAuthenticationException("Authentication is required.");
        }

        return UserMapper.ToResponse(user);
    }

    private AuthResponse CreateAuthResponse(User user)
    {
        var token = _authTokenService.CreateToken(user);

        return new AuthResponse(
            token.AccessToken,
            TokenType,
            token.ExpiresIn,
            UserMapper.ToResponse(user)
        );
    }

    private static string RequireText(string? value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ApplicationValidationException($"{fieldName} is required.");
        }

        return value.Trim();
    }

    private static string RequireEmail(string? email)
    {
        var normalizedEmail = RequireText(email, "Email").ToLowerInvariant();

        if (!normalizedEmail.Contains('@', StringComparison.Ordinal))
        {
            throw new ApplicationValidationException("Email must be valid.");
        }

        return normalizedEmail;
    }

    private static string RequirePassword(string? password)
    {
        var normalizedPassword = RequireText(password, "Password");

        if (normalizedPassword.Length < MinimumPasswordLength)
        {
            throw new ApplicationValidationException(
                $"Password must be at least {MinimumPasswordLength} characters."
            );
        }

        return normalizedPassword;
    }
}
