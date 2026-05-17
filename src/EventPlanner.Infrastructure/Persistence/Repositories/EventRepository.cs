using EventPlanner.Application.Events.Repositories;
using EventPlanner.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace EventPlanner.Infrastructure.Persistence.Repositories;

public sealed class EventRepository(EventPlannerDbContext dbContext) : IEventRepository
{
    private readonly EventPlannerDbContext _dbContext = dbContext;

    public async Task<IReadOnlyList<CalendarEvent>> GetAllAsync(
        CancellationToken cancellationToken
    )
    {
        return await _dbContext
            .CalendarEvents
            .AsNoTracking()
            .OrderBy(calendarEvent => calendarEvent.StartDateTime)
            .ToListAsync(cancellationToken);
    }

    public Task<CalendarEvent?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _dbContext.CalendarEvents.FirstOrDefaultAsync(
            calendarEvent => calendarEvent.Id == id,
            cancellationToken
        );
    }

    public async Task AddAsync(CalendarEvent calendarEvent, CancellationToken cancellationToken)
    {
        await _dbContext.CalendarEvents.AddAsync(calendarEvent, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
