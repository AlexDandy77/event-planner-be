using EventPlanner.Application.Common.Abstractions;
using EventPlanner.Application.Common.Dtos;
using EventPlanner.Application.Common.Exceptions;
using EventPlanner.Application.Events.Dtos;
using EventPlanner.Application.Events.Queries;
using EventPlanner.Application.Events.Repositories;
using EventPlanner.Application.Events.Services;
using EventPlanner.Application.Users.Repositories;
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
        var service = CreateService(repository);

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
        var service = CreateService(repository);

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
        var service = CreateService(repository);

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
    public async Task CreateEventAsync_ShouldUseCurrentUserAsOrganizer()
    {
        var repository = new FakeEventRepository();
        var currentUser = CreateUser("organizer@example.com", UserRole.Organizer);
        var service = CreateService(repository, currentUser);

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
        Assert.Equal(currentUser.Id, response.OrganizerId);
        Assert.Equal(FixedNow, response.CreatedAt);
        Assert.Single(repository.Events);
        Assert.Equal(1, repository.SaveChangesCallCount);
    }

    [Fact]
    public async Task CreateEventAsync_ShouldThrowAuthenticationException_WhenCurrentUserIsMissing()
    {
        var repository = new FakeEventRepository();
        var service = CreateService(repository);

        var createEvent = () =>
            service.CreateEventAsync(
                new CreateEventRequest
                {
                    Title = "Planning Session",
                    Category = "online",
                    StartDateTime = FixedNow.AddHours(1),
                    EndDateTime = FixedNow.AddHours(2),
                    Color = "#DC2626"
                },
                CancellationToken.None
            );

        await Assert.ThrowsAsync<ApplicationAuthenticationException>(createEvent);
        Assert.Empty(repository.Events);
        Assert.Equal(0, repository.SaveChangesCallCount);
    }

    [Fact]
    public async Task UpdateEventAsync_ShouldReturnNull_WhenEventDoesNotExist()
    {
        var repository = new FakeEventRepository();
        var service = CreateService(repository);

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
    public async Task UpdateEventAsync_ShouldThrowForbiddenException_WhenCurrentUserDoesNotOwnEvent()
    {
        var eventOwner = CreateUser("owner@example.com", UserRole.Organizer);
        var otherUser = CreateUser("other@example.com", UserRole.Organizer);
        var calendarEvent = CalendarEvent.Create(
            "Planning Session",
            null,
            EventCategory.Workshop,
            FixedNow.AddHours(1),
            FixedNow.AddHours(2),
            eventOwner.Id,
            FixedNow,
            "#2563EB"
        );
        var repository = new FakeEventRepository(calendarEvent);
        var service = CreateService(repository, otherUser, eventOwner);

        var updateEvent = () =>
            service.UpdateEventAsync(
                calendarEvent.Id,
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

        await Assert.ThrowsAsync<ApplicationForbiddenException>(updateEvent);
        Assert.Equal("Planning Session", calendarEvent.Title);
        Assert.Equal(0, repository.SaveChangesCallCount);
    }

    [Fact]
    public async Task UpdateEventAsync_ShouldAllowAdminToModifyAnyEvent()
    {
        var eventOwner = CreateUser("owner@example.com", UserRole.Organizer);
        var admin = CreateUser("admin@example.com", UserRole.Admin);
        var calendarEvent = CalendarEvent.Create(
            "Planning Session",
            null,
            EventCategory.Workshop,
            FixedNow.AddHours(1),
            FixedNow.AddHours(2),
            eventOwner.Id,
            FixedNow,
            "#2563EB"
        );
        var repository = new FakeEventRepository(calendarEvent);
        var service = CreateService(repository, admin, eventOwner);

        var response = await service.UpdateEventAsync(
            calendarEvent.Id,
            new UpdateEventRequest
            {
                Title = "Updated Session",
                Category = "hybrid",
                StartDateTime = FixedNow.AddHours(3),
                EndDateTime = FixedNow.AddHours(4),
                Color = "#DC2626"
            },
            CancellationToken.None
        );

        Assert.NotNull(response);
        Assert.Equal("Updated Session", response.Title);
        Assert.Equal("hybrid", response.Category);
        Assert.Equal("#DC2626", response.Color);
        Assert.Equal(1, repository.SaveChangesCallCount);
    }

    [Fact]
    public async Task DeleteEventAsync_ShouldSoftDeleteEvent_WhenCurrentUserOwnsEvent()
    {
        var eventOwner = CreateUser("owner@example.com", UserRole.Organizer);
        var calendarEvent = CalendarEvent.Create(
            "Planning Session",
            null,
            EventCategory.Workshop,
            FixedNow.AddHours(1),
            FixedNow.AddHours(2),
            eventOwner.Id,
            FixedNow,
            "#2563EB"
        );
        var repository = new FakeEventRepository(calendarEvent);
        var service = CreateService(
            repository,
            eventOwner,
            utcNow: FixedNow.AddHours(3)
        );

        var wasDeleted = await service.DeleteEventAsync(calendarEvent.Id, CancellationToken.None);

        Assert.True(wasDeleted);
        Assert.True(calendarEvent.IsDeleted);
        Assert.Equal(FixedNow.AddHours(3), calendarEvent.UpdatedAt);
        Assert.Equal(1, repository.SaveChangesCallCount);
    }

    [Fact]
    public async Task DeleteEventAsync_ShouldThrowForbiddenException_WhenCurrentUserDoesNotOwnEvent()
    {
        var eventOwner = CreateUser("owner@example.com", UserRole.Organizer);
        var otherUser = CreateUser("other@example.com", UserRole.Organizer);
        var calendarEvent = CalendarEvent.Create(
            "Planning Session",
            null,
            EventCategory.Workshop,
            FixedNow.AddHours(1),
            FixedNow.AddHours(2),
            eventOwner.Id,
            FixedNow,
            "#2563EB"
        );
        var repository = new FakeEventRepository(calendarEvent);
        var service = CreateService(repository, otherUser, eventOwner);

        var deleteEvent = () => service.DeleteEventAsync(calendarEvent.Id, CancellationToken.None);

        await Assert.ThrowsAsync<ApplicationForbiddenException>(deleteEvent);
        Assert.False(calendarEvent.IsDeleted);
        Assert.Equal(0, repository.SaveChangesCallCount);
    }

    private static EventService CreateService(
        FakeEventRepository eventRepository,
        User? currentUser = null,
        User? additionalUser = null,
        DateTimeOffset? utcNow = null
    )
    {
        var users = new[] { currentUser, additionalUser }.OfType<User>().ToArray();

        return new EventService(
            eventRepository,
            new FakeUserRepository(users),
            new FakeCurrentUserService(currentUser?.Id),
            new FixedClock(utcNow ?? FixedNow)
        );
    }

    private static User CreateUser(string email, UserRole role)
    {
        return User.Create("Test", "User", email, role, null, "hashed-password", FixedNow);
    }

    private sealed class FixedClock(DateTimeOffset utcNow) : IClock
    {
        public DateTimeOffset UtcNow { get; } = utcNow;
    }

    private sealed class FakeCurrentUserService(Guid? userId) : ICurrentUserService
    {
        public Guid? UserId { get; } = userId;
    }

    private sealed class FakeUserRepository(params User[] users) : IUserRepository
    {
        private readonly List<User> _users = [.. users];

        public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return Task.FromResult(_users.FirstOrDefault(user => user.Id == id));
        }

        public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
        {
            var normalizedEmail = email.Trim().ToLowerInvariant();

            return Task.FromResult(
                _users.FirstOrDefault(user => user.Email == normalizedEmail)
            );
        }

        public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken)
        {
            var normalizedEmail = email.Trim().ToLowerInvariant();

            return Task.FromResult(_users.Any(user => user.Email == normalizedEmail));
        }

        public Task AddAsync(User user, CancellationToken cancellationToken)
        {
            _users.Add(user);

            return Task.CompletedTask;
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
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
