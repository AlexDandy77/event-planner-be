using EventPlanner.Application.Events.Dtos;

namespace EventPlanner.Application.Events.Services;

public interface IEventService
{
    Task<IReadOnlyList<EventResponse>> GetEventsAsync(
        EventQueryParameters queryParameters,
        CancellationToken cancellationToken
    );

    Task<EventResponse> GetEventByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<EventResponse> CreateEventAsync(
        CreateEventRequest request,
        CancellationToken cancellationToken
    );

    Task<EventResponse> UpdateEventAsync(
        Guid id,
        UpdateEventRequest request,
        CancellationToken cancellationToken
    );

    Task DeleteEventAsync(Guid id, CancellationToken cancellationToken);
}
