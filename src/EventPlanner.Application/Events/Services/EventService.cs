using EventPlanner.Application.Common.Abstractions;
using EventPlanner.Application.Common.Dtos;
using EventPlanner.Application.Common.Exceptions;
using EventPlanner.Application.Common.Validation;
using EventPlanner.Application.Events.Dtos;
using EventPlanner.Application.Events.Mapping;
using EventPlanner.Application.Events.Queries;
using EventPlanner.Application.Events.Repositories;
using EventPlanner.Application.Users.Repositories;
using EventPlanner.Domain.Entities;
using EventPlanner.Domain.Enums;

namespace EventPlanner.Application.Events.Services;

public sealed class EventService(
    IEventRepository eventRepository,
    IUserRepository userRepository,
    ICurrentUserService currentUserService,
    IClock clock,
    IRequestValidator<EventQueryParameters> queryParametersValidator,
    IRequestValidator<CreateEventRequest> createEventRequestValidator,
    IRequestValidator<UpdateEventRequest> updateEventRequestValidator
) : IEventService
{
    private readonly IClock _clock = clock;
    private readonly IRequestValidator<CreateEventRequest> _createEventRequestValidator =
        createEventRequestValidator;
    private readonly ICurrentUserService _currentUserService = currentUserService;
    private readonly IEventRepository _eventRepository = eventRepository;
    private readonly IRequestValidator<EventQueryParameters> _queryParametersValidator =
        queryParametersValidator;
    private readonly IRequestValidator<UpdateEventRequest> _updateEventRequestValidator =
        updateEventRequestValidator;
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<IReadOnlyList<EventResponse>> GetEventsAsync(
        EventQueryParameters queryParameters,
        CancellationToken cancellationToken
    )
    {
        _queryParametersValidator.Validate(queryParameters);

        var query = BuildQuery(queryParameters);
        var events = await _eventRepository.GetAllAsync(query, cancellationToken);

        return [.. events.Select(EventMapper.ToResponse)];
    }

    public async Task<EventResponse> GetEventByIdAsync(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        var calendarEvent = await _eventRepository.GetByIdAsync(id, cancellationToken);

        if (calendarEvent is null)
        {
            throw EventNotFound(id);
        }

        return EventMapper.ToResponse(calendarEvent);
    }

    public async Task<EventResponse> CreateEventAsync(
        CreateEventRequest request,
        CancellationToken cancellationToken
    )
    {
        _createEventRequestValidator.Validate(request);

        var currentUser = await GetCurrentActiveUserAsync(cancellationToken);
        var category = EventCategoryMapper.ToDomainValue(request.Category);
        var createdAt = _clock.UtcNow;

        var calendarEvent = CalendarEvent.Create(
            RequireText(request.Title, "Event title"),
            request.Description,
            category,
            request.StartDateTime,
            request.EndDateTime,
            currentUser.Id,
            createdAt,
            RequireText(request.Color, "Event color")
        );

        await _eventRepository.AddAsync(calendarEvent, cancellationToken);
        await _eventRepository.SaveChangesAsync(cancellationToken);

        return EventMapper.ToResponse(calendarEvent);
    }

    public async Task<EventResponse> UpdateEventAsync(
        Guid id,
        UpdateEventRequest request,
        CancellationToken cancellationToken
    )
    {
        _updateEventRequestValidator.Validate(request);

        var calendarEvent = await _eventRepository.GetByIdAsync(id, cancellationToken);

        if (calendarEvent is null)
        {
            throw EventNotFound(id);
        }

        var currentUser = await GetCurrentActiveUserAsync(cancellationToken);
        EnsureCanModifyEvent(currentUser, calendarEvent);

        var category = EventCategoryMapper.ToDomainValue(request.Category);

        calendarEvent.UpdateDetails(
            RequireText(request.Title, "Event title"),
            request.Description,
            category,
            request.StartDateTime,
            request.EndDateTime,
            RequireText(request.Color, "Event color"),
            _clock.UtcNow
        );

        await _eventRepository.SaveChangesAsync(cancellationToken);

        return EventMapper.ToResponse(calendarEvent);
    }

    public async Task DeleteEventAsync(Guid id, CancellationToken cancellationToken)
    {
        var calendarEvent = await _eventRepository.GetByIdAsync(id, cancellationToken);

        if (calendarEvent is null)
        {
            throw EventNotFound(id);
        }

        var currentUser = await GetCurrentActiveUserAsync(cancellationToken);
        EnsureCanModifyEvent(currentUser, calendarEvent);

        calendarEvent.Delete(_clock.UtcNow);

        await _eventRepository.SaveChangesAsync(cancellationToken);
    }

    private static ApplicationNotFoundException EventNotFound(Guid id)
    {
        return new ApplicationNotFoundException($"Event with id '{id}' was not found.");
    }

    private async Task<User> GetCurrentActiveUserAsync(CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        if (!currentUserId.HasValue)
        {
            throw new ApplicationAuthenticationException("Authentication is required.");
        }

        var currentUser = await _userRepository.GetByIdAsync(currentUserId.Value, cancellationToken);

        if (currentUser is null || !currentUser.IsActive)
        {
            throw new ApplicationAuthenticationException("Authentication is required.");
        }

        return currentUser;
    }

    private static void EnsureCanModifyEvent(User currentUser, CalendarEvent calendarEvent)
    {
        if (currentUser.Role == UserRole.Admin || calendarEvent.OrganizerId == currentUser.Id)
        {
            return;
        }

        throw new ApplicationForbiddenException("You cannot modify this event.");
    }

    private static string RequireText(string? value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ApplicationValidationException($"{fieldName} is required.");
        }

        return value;
    }

    private static EventQuery BuildQuery(EventQueryParameters queryParameters)
    {
        if (
            queryParameters.From.HasValue
            && queryParameters.To.HasValue
            && queryParameters.From >= queryParameters.To
        )
        {
            throw new ApplicationValidationException(
                "Query parameter 'from' must be earlier than 'to'."
            );
        }

        EventCategory? category = string.IsNullOrWhiteSpace(queryParameters.Category)
            ? null
            : EventCategoryMapper.ToDomainValue(queryParameters.Category);

        return new EventQuery(
            queryParameters.From,
            queryParameters.To,
            category,
            NormalizeSearch(queryParameters.Search),
            NormalizeSortBy(queryParameters.SortBy),
            NormalizeSortDirection(queryParameters.SortDirection)
        );
    }

    private static string? NormalizeSearch(string? search)
    {
        return string.IsNullOrWhiteSpace(search) ? null : search.Trim();
    }

    private static string NormalizeSortBy(string? sortBy)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
        {
            return EventSortFields.StartDateTime;
        }

        if (sortBy.Equals(EventSortFields.StartDateTime, StringComparison.OrdinalIgnoreCase))
        {
            return EventSortFields.StartDateTime;
        }

        if (sortBy.Equals(EventSortFields.Title, StringComparison.OrdinalIgnoreCase))
        {
            return EventSortFields.Title;
        }

        if (sortBy.Equals(EventSortFields.CreatedAt, StringComparison.OrdinalIgnoreCase))
        {
            return EventSortFields.CreatedAt;
        }

        throw new ApplicationValidationException($"Sort field '{sortBy}' is not supported.");
    }

    private static string NormalizeSortDirection(string? sortDirection)
    {
        if (string.IsNullOrWhiteSpace(sortDirection))
        {
            return SortDirections.Asc;
        }

        if (sortDirection.Equals(SortDirections.Asc, StringComparison.OrdinalIgnoreCase))
        {
            return SortDirections.Asc;
        }

        if (sortDirection.Equals(SortDirections.Desc, StringComparison.OrdinalIgnoreCase))
        {
            return SortDirections.Desc;
        }

        throw new ApplicationValidationException(
            $"Sort direction '{sortDirection}' is not supported."
        );
    }
}
