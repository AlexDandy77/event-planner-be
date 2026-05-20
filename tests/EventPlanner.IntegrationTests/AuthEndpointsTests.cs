using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

using EventPlanner.Application.Auth.Dtos;
using EventPlanner.Application.Users.Dtos;
using EventPlanner.IntegrationTests.Support;

using Microsoft.AspNetCore.Mvc;

namespace EventPlanner.IntegrationTests;

public sealed class AuthEndpointsTests
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    [Fact]
    public async Task Register_ShouldCreateUserAndReturnToken()
    {
        using var factory = new EventPlannerApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/auth/register",
            new RegisterRequest
            {
                FirstName = "Integration",
                LastName = "User",
                Email = "Integration.User@Example.COM",
                Password = "Password123!",
                Phone = "+37360002222"
            },
            JsonOptions
        );

        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>(JsonOptions);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(authResponse);
        Assert.False(string.IsNullOrWhiteSpace(authResponse.AccessToken));
        Assert.Equal("integration.user@example.com", authResponse.User.Email);
        Assert.Equal("organizer", authResponse.User.Role);
    }

    [Fact]
    public async Task Register_ShouldReturnConflictProblem_WhenEmailAlreadyExists()
    {
        using var factory = new EventPlannerApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/auth/register",
            new RegisterRequest
            {
                FirstName = "Existing",
                LastName = "Organizer",
                Email = "ORGANIZER@example.com",
                Password = "Password123!"
            },
            JsonOptions
        );

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>(JsonOptions);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(problem);
        Assert.Equal(409, problem.Status);
        Assert.Equal("Conflict", problem.Title);
    }

    [Fact]
    public async Task LoginAndMe_ShouldReturnCurrentUser_WhenTokenIsProvided()
    {
        using var factory = new EventPlannerApiFactory();
        using var client = factory.CreateClient();

        var login = await client.LoginAsAdminAsync();
        client.UseBearerToken(login.AccessToken);

        var response = await client.GetAsync("/api/auth/me");
        var user = await response.Content.ReadFromJsonAsync<UserResponse>(JsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(user);
        Assert.Equal("admin@example.com", user.Email);
        Assert.Equal("admin", user.Role);
    }
}
