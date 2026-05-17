using EventPlanner.Application.Common.Dtos;
using EventPlanner.Application.Events.Dtos;
using EventPlanner.Application.Events.Queries;
using EventPlanner.Application.Events.Repositories;
using EventPlanner.Domain.Entities;

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
        var events = _dbContext.CalendarEvents.AsNoTracking();

        if (query.From.HasValue)
        {
            events = events.Where(calendarEvent => calendarEvent.EndDateTime > query.From.Value);
        }

        if (query.To.HasValue)
        {
            events = events.Where(calendarEvent => calendarEvent.StartDateTime < query.To.Value);
        }

        if (query.Category.HasValue)
        {
            events = events.Where(calendarEvent => calendarEvent.Category == query.Category.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.ToLowerInvariant();

            events = events.Where(calendarEvent =>
                calendarEvent.Title.ToLower().Contains(search)
                || (
                    calendarEvent.Description != null
                    && calendarEvent.Description.ToLower().Contains(search)
                )
            );
        }

        events = ApplySorting(events, query);

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

    private static IQueryable<CalendarEvent> ApplySorting(
        IQueryable<CalendarEvent> events,
        EventQuery query
    )
    {
        var descending = query.SortDirection == SortDirections.Desc;

        return query.SortBy switch
        {
            EventSortFields.Title => descending
                ? events
                    .OrderByDescending(calendarEvent => calendarEvent.Title)
                    .ThenBy(calendarEvent => calendarEvent.StartDateTime)
                : events
                    .OrderBy(calendarEvent => calendarEvent.Title)
                    .ThenBy(calendarEvent => calendarEvent.StartDateTime),
            EventSortFields.CreatedAt => descending
                ? events
                    .OrderByDescending(calendarEvent => calendarEvent.CreatedAt)
                    .ThenBy(calendarEvent => calendarEvent.StartDateTime)
                : events
                    .OrderBy(calendarEvent => calendarEvent.CreatedAt)
                    .ThenBy(calendarEvent => calendarEvent.StartDateTime),
            _ => descending
                ? events
                    .OrderByDescending(calendarEvent => calendarEvent.StartDateTime)
                    .ThenBy(calendarEvent => calendarEvent.Title)
                : events
                    .OrderBy(calendarEvent => calendarEvent.StartDateTime)
                    .ThenBy(calendarEvent => calendarEvent.Title)
        };
    }
}
