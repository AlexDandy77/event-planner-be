using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

using EventPlanner.Application.Events.Dtos;
using EventPlanner.IntegrationTests.Support;

using Microsoft.AspNetCore.Mvc;

namespace EventPlanner.IntegrationTests;

public sealed class EventEndpointsTests
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    [Fact]
    public async Task GetEvents_ShouldReturnSeededEvents()
    {
        using var factory = new EventPlannerApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/events");
        var body = await response.Content.ReadAsStringAsync();

        Assert.True(
            response.IsSuccessStatusCode,
            $"Expected success status code, got {response.StatusCode}. Body: {body}"
        );

        var events = await response.Content.ReadFromJsonAsync<IReadOnlyList<EventResponse>>(
            JsonOptions
        );

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(events);
        Assert.Contains(events, calendarEvent => calendarEvent.Title == "Tech Career Fair 2026");
    }

    [Fact]
    public async Task GetEvents_ShouldReturnValidationProblem_WhenQueryIsInvalid()
    {
        using var factory = new EventPlannerApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync(
            "/api/events?from=2026-05-21T12:00:00Z&to=2026-05-21T11:00:00Z&sortBy=priority"
        );
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>(
            JsonOptions
        );

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(problem);
        Assert.Contains("from", problem.Errors.Keys);
        Assert.Contains("to", problem.Errors.Keys);
        Assert.Contains("sortBy", problem.Errors.Keys);
    }

    [Fact]
    public async Task CreateEvent_ShouldReturnUnauthorizedProblem_WhenTokenIsMissing()
    {
        using var factory = new EventPlannerApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/events",
            CreateEventRequest(),
            JsonOptions
        );
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>(JsonOptions);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(problem);
        Assert.Equal(401, problem.Status);
    }

    [Fact]
    public async Task CreateUpdateDeleteEvent_ShouldWork_WhenUserIsAuthenticated()
    {
        using var factory = new EventPlannerApiFactory();
        using var client = factory.CreateClient();

        var login = await client.LoginAsAdminAsync();
        client.UseBearerToken(login.AccessToken);

        var createResponse = await client.PostAsJsonAsync(
            "/api/events",
            CreateEventRequest(),
            JsonOptions
        );
        var createdEvent = await createResponse.Content.ReadFromJsonAsync<EventResponse>(
            JsonOptions
        );

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        Assert.NotNull(createdEvent);
        Assert.Equal(login.User.Id, createdEvent.OrganizerId);

        var updateResponse = await client.PutAsJsonAsync(
            $"/api/events/{createdEvent.Id}",
            new UpdateEventRequest
            {
                Title = "Updated Integration Event",
                Description = "Updated from integration tests.",
                Category = "hybrid",
                StartDateTime = DateTimeOffset.Parse("2026-06-01T15:00:00Z"),
                EndDateTime = DateTimeOffset.Parse("2026-06-01T16:30:00Z"),
                Color = "#2563EB"
            },
            JsonOptions
        );
        var updatedEvent = await updateResponse.Content.ReadFromJsonAsync<EventResponse>(
            JsonOptions
        );

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        Assert.NotNull(updatedEvent);
        Assert.Equal("Updated Integration Event", updatedEvent.Title);
        Assert.Equal("hybrid", updatedEvent.Category);

        var deleteResponse = await client.DeleteAsync($"/api/events/{createdEvent.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getDeletedResponse = await client.GetAsync($"/api/events/{createdEvent.Id}");
        var problem = await getDeletedResponse.Content.ReadFromJsonAsync<ProblemDetails>(
            JsonOptions
        );

        Assert.Equal(HttpStatusCode.NotFound, getDeletedResponse.StatusCode);
        Assert.Equal("application/problem+json", getDeletedResponse.Content.Headers.ContentType?.MediaType);
        Assert.NotNull(problem);
        Assert.Equal(404, problem.Status);
    }

    private static CreateEventRequest CreateEventRequest()
    {
        return new CreateEventRequest
        {
            Title = "Integration Event",
            Description = "Created from integration tests.",
            Category = "online",
            StartDateTime = DateTimeOffset.Parse("2026-06-01T12:00:00Z"),
            EndDateTime = DateTimeOffset.Parse("2026-06-01T13:00:00Z"),
            Color = "#DC2626"
        };
    }
}
