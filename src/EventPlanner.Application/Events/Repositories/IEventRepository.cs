using EventPlanner.Application.Events.Queries;
using EventPlanner.Domain.Entities;

namespace EventPlanner.Application.Events.Repositories;

public interface IEventRepository
{
    Task<IReadOnlyList<CalendarEvent>> GetAllAsync(
        EventQuery query,
        CancellationToken cancellationToken
    );

    Task<CalendarEvent?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task AddAsync(CalendarEvent calendarEvent, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
