using EventPlanner.Application.Common.Dtos;
using EventPlanner.Application.Events.Dtos;
using EventPlanner.Domain.Entities;
using EventPlanner.Domain.Enums;

namespace EventPlanner.Infrastructure.Persistence.Queries;

public static class EventQueryableExtensions
{
    public static IQueryable<CalendarEvent> ApplyDateRange(
        this IQueryable<CalendarEvent> events,
        DateTimeOffset? from,
        DateTimeOffset? to
    )
    {
        if (from.HasValue)
        {
            events = events.Where(calendarEvent => calendarEvent.EndDateTime > from.Value);
        }

        if (to.HasValue)
        {
            events = events.Where(calendarEvent => calendarEvent.StartDateTime < to.Value);
        }

        return events;
    }

    public static IQueryable<CalendarEvent> ApplyCategoryFilter(
        this IQueryable<CalendarEvent> events,
        EventCategory? category
    )
    {
        return category.HasValue
            ? events.Where(calendarEvent => calendarEvent.Category == category.Value)
            : events;
    }

    public static IQueryable<CalendarEvent> ApplySearch(
        this IQueryable<CalendarEvent> events,
        string? search
    )
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            return events;
        }

        var normalizedSearch = search.ToLowerInvariant();

        return events.Where(calendarEvent =>
            calendarEvent.Title.ToLower().Contains(normalizedSearch)
            || (
                calendarEvent.Description != null
                && calendarEvent.Description.ToLower().Contains(normalizedSearch)
            )
        );
    }

    public static IQueryable<CalendarEvent> ApplySorting(
        this IQueryable<CalendarEvent> events,
        string sortBy,
        string sortDirection
    )
    {
        var descending = sortDirection == SortDirections.Desc;

        return sortBy switch
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
