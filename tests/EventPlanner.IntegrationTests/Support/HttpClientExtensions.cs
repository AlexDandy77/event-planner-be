using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

using EventPlanner.Application.Auth.Dtos;

namespace EventPlanner.IntegrationTests.Support;

internal static class HttpClientExtensions
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public static async Task<AuthResponse> LoginAsAdminAsync(this HttpClient client)
    {
        var response = await client.PostAsJsonAsync(
            "/api/auth/login",
            new LoginRequest
            {
                Email = "admin@example.com",
                Password = "Password123!"
            },
            JsonOptions
        );
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<AuthResponse>(JsonOptions)
            ?? throw new InvalidOperationException("Login response could not be deserialized.");
    }

    public static void UseBearerToken(this HttpClient client, string accessToken)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            accessToken
        );
    }
}
