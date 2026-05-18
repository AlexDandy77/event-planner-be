using EventPlanner.Application.Events.Queries;
using EventPlanner.Application.Events.Repositories;
using EventPlanner.Domain.Entities;
using EventPlanner.Infrastructure.Persistence.Queries;

using Microsoft.EntityFrameworkCore;

namespace EventPlanner.Infrastructure.Persistence.Repositories;

public sealed class EventRepository(EventPlannerDbContext dbContext) : IEventRepository
{
    private readonly EventPlannerDbContext _dbContext = dbContext;

    public async Task<IReadOnlyList<CalendarEvent>> GetAllAsync(
        EventQuery query,
        CancellationToken cancellationToken
    )
    {
        var events = _dbContext
            .CalendarEvents
            .AsNoTracking()
            .ApplyDateRange(query.From, query.To)
            .ApplyCategoryFilter(query.Category)
            .ApplySearch(query.Search)
            .ApplySorting(query.SortBy, query.SortDirection);

        return await events.ToListAsync(cancellationToken);
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
