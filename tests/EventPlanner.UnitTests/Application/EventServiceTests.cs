using EventPlanner.Application.Common.Abstractions;
using EventPlanner.Application.Common.Dtos;
using EventPlanner.Application.Common.Exceptions;
using EventPlanner.Application.Events.Dtos;
using EventPlanner.Application.Events.Queries;
using EventPlanner.Application.Events.Repositories;
using EventPlanner.Application.Events.Services;
using EventPlanner.Domain.Entities;
using EventPlanner.Domain.Enums;

namespace EventPlanner.UnitTests.Application;

public sealed class EventServiceTests
{
    private static readonly DateTimeOffset FixedNow = DateTimeOffset.Parse(
        "2026-05-20T10:00:00Z"
    );

    [Fact]
    public async Task GetEventsAsync_ShouldNormalizeQueryParameters()
    {
        var repository = new FakeEventRepository();
        var service = new EventService(repository, new FixedClock(FixedNow));

        await service.GetEventsAsync(
            new EventQueryParameters
            {
                From = FixedNow,
                To = FixedNow.AddDays(7),
                Category = "WORKSHOP",
                Search = " planning ",
                SortBy = "TITLE",
                SortDirection = "DESC"
            },
            CancellationToken.None
        );

        Assert.NotNull(repository.LastQuery);
        Assert.Equal(FixedNow, repository.LastQuery.From);
        Assert.Equal(FixedNow.AddDays(7), repository.LastQuery.To);
        Assert.Equal(EventCategory.Workshop, repository.LastQuery.Category);
        Assert.Equal("planning", repository.LastQuery.Search);
        Assert.Equal(EventSortFields.Title, repository.LastQuery.SortBy);
        Assert.Equal(SortDirections.Desc, repository.LastQuery.SortDirection);
    }

    [Fact]
    public async Task GetEventsAsync_ShouldThrow_WhenQueryDateRangeIsInvalid()
    {
        var repository = new FakeEventRepository();
        var service = new EventService(repository, new FixedClock(FixedNow));

        var getEvents = () =>
            service.GetEventsAsync(
                new EventQueryParameters
                {
                    From = FixedNow.AddDays(1),
                    To = FixedNow
                },
                CancellationToken.None
            );

        await Assert.ThrowsAsync<ApplicationValidationException>(getEvents);
    }

    [Fact]
    public async Task GetEventsAsync_ShouldThrow_WhenSortFieldIsUnsupported()
    {
        var repository = new FakeEventRepository();
        var service = new EventService(repository, new FixedClock(FixedNow));

        var getEvents = () =>
            service.GetEventsAsync(
                new EventQueryParameters
                {
                    SortBy = "priority"
                },
                CancellationToken.None
            );

        await Assert.ThrowsAsync<ApplicationValidationException>(getEvents);
    }

    [Fact]
    public async Task CreateEventAsync_ShouldCreateEventWithDevelopmentOrganizer()
    {
        var repository = new FakeEventRepository();
        var service = new EventService(repository, new FixedClock(FixedNow));

        var response = await service.CreateEventAsync(
            new CreateEventRequest
            {
                Title = "Planning Session",
                Description = "Discuss the roadmap.",
                Category = "online",
                StartDateTime = FixedNow.AddHours(1),
                EndDateTime = FixedNow.AddHours(2),
                Color = "#DC2626"
            },
            CancellationToken.None
        );

        Assert.Equal("Planning Session", response.Title);
        Assert.Equal("online", response.Category);
        Assert.Equal(Guid.Parse("8a5edca7-8f80-4ed6-8b91-01863782b51b"), response.OrganizerId);
        Assert.Equal(FixedNow, response.CreatedAt);
        Assert.Single(repository.Events);
        Assert.Equal(1, repository.SaveChangesCallCount);
    }

    [Fact]
    public async Task UpdateEventAsync_ShouldReturnNull_WhenEventDoesNotExist()
    {
        var repository = new FakeEventRepository();
        var service = new EventService(repository, new FixedClock(FixedNow));

        var response = await service.UpdateEventAsync(
            Guid.NewGuid(),
            new UpdateEventRequest
            {
                Title = "Updated Session",
                Category = "workshop",
                StartDateTime = FixedNow.AddHours(1),
                EndDateTime = FixedNow.AddHours(2),
                Color = "#2563EB"
            },
            CancellationToken.None
        );

        Assert.Null(response);
        Assert.Equal(0, repository.SaveChangesCallCount);
    }

    [Fact]
    public async Task DeleteEventAsync_ShouldSoftDeleteEvent()
    {
        var calendarEvent = CalendarEvent.Create(
            "Planning Session",
            null,
            EventCategory.Workshop,
            FixedNow.AddHours(1),
            FixedNow.AddHours(2),
            Guid.NewGuid(),
            FixedNow,
            "#2563EB"
        );
        var repository = new FakeEventRepository(calendarEvent);
        var service = new EventService(repository, new FixedClock(FixedNow.AddHours(3)));

        var wasDeleted = await service.DeleteEventAsync(calendarEvent.Id, CancellationToken.None);

        Assert.True(wasDeleted);
        Assert.True(calendarEvent.IsDeleted);
        Assert.Equal(FixedNow.AddHours(3), calendarEvent.UpdatedAt);
        Assert.Equal(1, repository.SaveChangesCallCount);
    }

    private sealed class FixedClock(DateTimeOffset utcNow) : IClock
    {
        public DateTimeOffset UtcNow { get; } = utcNow;
    }

    private sealed class FakeEventRepository(params CalendarEvent[] events) : IEventRepository
    {
        public List<CalendarEvent> Events { get; } = [.. events];

        public EventQuery? LastQuery { get; private set; }

        public int SaveChangesCallCount { get; private set; }

        public Task<IReadOnlyList<CalendarEvent>> GetAllAsync(
            EventQuery query,
            CancellationToken cancellationToken
        )
        {
            LastQuery = query;

            var events = Events
                .Where(calendarEvent => !calendarEvent.IsDeleted)
                .OrderBy(calendarEvent => calendarEvent.StartDateTime)
                .ToArray();

            return Task.FromResult<IReadOnlyList<CalendarEvent>>(events);
        }

        public Task<CalendarEvent?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var calendarEvent = Events.FirstOrDefault(calendarEvent =>
                calendarEvent.Id == id && !calendarEvent.IsDeleted
            );

            return Task.FromResult(calendarEvent);
        }

        public Task AddAsync(CalendarEvent calendarEvent, CancellationToken cancellationToken)
        {
            Events.Add(calendarEvent);

            return Task.CompletedTask;
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            SaveChangesCallCount++;

            return Task.CompletedTask;
        }
    }
}
