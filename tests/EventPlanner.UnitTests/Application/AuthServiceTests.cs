using EventPlanner.Application.Auth.Abstractions;
using EventPlanner.Application.Auth.Dtos;
using EventPlanner.Application.Auth.Models;
using EventPlanner.Application.Auth.Services;
using EventPlanner.Application.Common.Abstractions;
using EventPlanner.Application.Common.Exceptions;
using EventPlanner.Application.Users.Repositories;
using EventPlanner.Domain.Entities;
using EventPlanner.Domain.Enums;

namespace EventPlanner.UnitTests.Application;

public sealed class AuthServiceTests
{
    private static readonly DateTimeOffset FixedNow = DateTimeOffset.Parse(
        "2026-05-21T09:00:00Z"
    );

    [Fact]
    public async Task RegisterAsync_ShouldCreateUserAndReturnAuthResponse()
    {
        var userRepository = new FakeUserRepository();
        var service = CreateService(userRepository);

        var response = await service.RegisterAsync(
            new RegisterRequest
            {
                FirstName = "New",
                LastName = "Organizer",
                Email = "New.Organizer@Example.COM",
                Password = "Password123!",
                Phone = "+37360001111"
            },
            CancellationToken.None
        );

        Assert.Equal("fake-jwt-token", response.AccessToken);
        Assert.Equal("Bearer", response.TokenType);
        Assert.Equal(3600, response.ExpiresIn);
        Assert.Equal("new.organizer@example.com", response.User.Email);
        Assert.Equal("organizer", response.User.Role);
        Assert.Single(userRepository.Users);
        Assert.Equal("hashed:Password123!", userRepository.Users[0].PasswordHash);
        Assert.Equal(1, userRepository.SaveChangesCallCount);
    }

    [Fact]
    public async Task RegisterAsync_ShouldThrowConflict_WhenEmailAlreadyExists()
    {
        var existingUser = CreateUser("admin@example.com", UserRole.Admin);
        var service = CreateService(new FakeUserRepository(existingUser));

        var register = () =>
            service.RegisterAsync(
                new RegisterRequest
                {
                    FirstName = "Existing",
                    LastName = "User",
                    Email = "ADMIN@example.com",
                    Password = "Password123!"
                },
                CancellationToken.None
            );

        await Assert.ThrowsAsync<ApplicationConflictException>(register);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnAuthResponse_WhenCredentialsAreValid()
    {
        var existingUser = CreateUser("admin@example.com", UserRole.Admin);
        var service = CreateService(new FakeUserRepository(existingUser));

        var response = await service.LoginAsync(
            new LoginRequest
            {
                Email = " ADMIN@example.com ",
                Password = "Password123!"
            },
            CancellationToken.None
        );

        Assert.Equal("fake-jwt-token", response.AccessToken);
        Assert.Equal(existingUser.Id, response.User.Id);
        Assert.Equal("admin", response.User.Role);
    }

    [Fact]
    public async Task LoginAsync_ShouldThrowAuthenticationException_WhenPasswordIsInvalid()
    {
        var existingUser = CreateUser("admin@example.com", UserRole.Admin);
        var service = CreateService(new FakeUserRepository(existingUser));

        var login = () =>
            service.LoginAsync(
                new LoginRequest
                {
                    Email = "admin@example.com",
                    Password = "wrong-password"
                },
                CancellationToken.None
            );

        await Assert.ThrowsAsync<ApplicationAuthenticationException>(login);
    }

    [Fact]
    public async Task GetCurrentUserAsync_ShouldReturnCurrentUser()
    {
        var existingUser = CreateUser("organizer@example.com", UserRole.Organizer);
        var service = CreateService(
            new FakeUserRepository(existingUser),
            currentUserId: existingUser.Id
        );

        var response = await service.GetCurrentUserAsync(CancellationToken.None);

        Assert.Equal(existingUser.Id, response.Id);
        Assert.Equal("organizer@example.com", response.Email);
    }

    [Fact]
    public async Task GetCurrentUserAsync_ShouldThrowAuthenticationException_WhenUserIdIsMissing()
    {
        var service = CreateService(new FakeUserRepository());

        var getCurrentUser = () => service.GetCurrentUserAsync(CancellationToken.None);

        await Assert.ThrowsAsync<ApplicationAuthenticationException>(getCurrentUser);
    }

    private static AuthService CreateService(
        FakeUserRepository userRepository,
        Guid? currentUserId = null
    )
    {
        return new AuthService(
            userRepository,
            new FakePasswordHasher(),
            new FakeAuthTokenService(),
            new FakeCurrentUserService(currentUserId),
            new FixedClock(FixedNow)
        );
    }

    private static User CreateUser(string email, UserRole role)
    {
        return User.Create(
            "Test",
            "User",
            email,
            role,
            null,
            "hashed:Password123!",
            FixedNow
        );
    }

    private sealed class FakeUserRepository(params User[] users) : IUserRepository
    {
        public List<User> Users { get; } = [.. users];

        public int SaveChangesCallCount { get; private set; }

        public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return Task.FromResult(Users.FirstOrDefault(user => user.Id == id));
        }

        public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
        {
            var normalizedEmail = email.Trim().ToLowerInvariant();

            return Task.FromResult(
                Users.FirstOrDefault(user => user.Email == normalizedEmail)
            );
        }

        public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken)
        {
            var normalizedEmail = email.Trim().ToLowerInvariant();

            return Task.FromResult(Users.Any(user => user.Email == normalizedEmail));
        }

        public Task AddAsync(User user, CancellationToken cancellationToken)
        {
            Users.Add(user);

            return Task.CompletedTask;
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            SaveChangesCallCount++;

            return Task.CompletedTask;
        }
    }

    private sealed class FakePasswordHasher : IPasswordHasher
    {
        public string Hash(string password)
        {
            return $"hashed:{password}";
        }

        public bool Verify(string password, string passwordHash)
        {
            return passwordHash == $"hashed:{password}";
        }
    }

    private sealed class FakeAuthTokenService : IAuthTokenService
    {
        public AuthToken CreateToken(User user)
        {
            return new AuthToken("fake-jwt-token", 3600);
        }
    }

    private sealed class FakeCurrentUserService(Guid? userId) : ICurrentUserService
    {
        public Guid? UserId { get; } = userId;
    }

    private sealed class FixedClock(DateTimeOffset utcNow) : IClock
    {
        public DateTimeOffset UtcNow { get; } = utcNow;
    }
}
