using EventPlanner.Application.Events.Dtos;
using EventPlanner.Domain.Entities;

namespace EventPlanner.Application.Events.Mapping;

internal static class EventMapper
{
    public static EventResponse ToResponse(CalendarEvent calendarEvent)
    {
        return new EventResponse(
            calendarEvent.Id,
            calendarEvent.Title,
            calendarEvent.Description,
            EventCategoryMapper.ToContractValue(calendarEvent.Category),
            calendarEvent.StartDateTime,
            calendarEvent.EndDateTime,
            calendarEvent.OrganizerId,
            calendarEvent.CreatedAt,
            calendarEvent.UpdatedAt,
            calendarEvent.Color,
            calendarEvent.IsDeleted
        );
    }
}
