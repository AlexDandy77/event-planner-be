using EventPlanner.Application.Events.Dtos;

namespace EventPlanner.Application.Events.Services;

public interface IEventService
{
    Task<IReadOnlyList<EventResponse>> GetEventsAsync(CancellationToken cancellationToken);

    Task<EventResponse?> GetEventByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<EventResponse> CreateEventAsync(
        CreateEventRequest request,
        CancellationToken cancellationToken
    );

    Task<EventResponse?> UpdateEventAsync(
        Guid id,
        UpdateEventRequest request,
        CancellationToken cancellationToken
    );

    Task<bool> DeleteEventAsync(Guid id, CancellationToken cancellationToken);
}
