using EventPlanner.Application.Common.Dtos;
using EventPlanner.Application.Events.Dtos;
using EventPlanner.Domain.Entities;
using EventPlanner.Domain.Enums;
using EventPlanner.Infrastructure.Persistence.Queries;

namespace EventPlanner.UnitTests.Infrastructure;

public sealed class EventQueryableExtensionsTests
{
    private static readonly DateTimeOffset BaseTime = DateTimeOffset.Parse(
        "2026-05-20T10:00:00Z"
    );

    [Fact]
    public void ApplyDateRange_ShouldReturnEventsThatOverlapRange()
    {
        var events = new[]
        {
            CreateEvent("Ends before range", BaseTime.AddHours(-3), BaseTime.AddHours(-1)),
            CreateEvent("Overlaps range start", BaseTime.AddHours(-1), BaseTime.AddMinutes(30)),
            CreateEvent("Inside range", BaseTime.AddMinutes(30), BaseTime.AddHours(1)),
            CreateEvent("Overlaps range end", BaseTime.AddHours(1), BaseTime.AddHours(3)),
            CreateEvent("Starts after range", BaseTime.AddHours(2), BaseTime.AddHours(3))
        };

        var result = events
            .AsQueryable()
            .ApplyDateRange(BaseTime, BaseTime.AddHours(2))
            .Select(calendarEvent => calendarEvent.Title)
            .ToArray();

        Assert.Equal(
            ["Overlaps range start", "Inside range", "Overlaps range end"],
            result
        );
    }

    [Fact]
    public void ApplyCategoryFilter_ShouldReturnOnlyMatchingCategory()
    {
        var events = new[]
        {
            CreateEvent("Online event", BaseTime, BaseTime.AddHours(1), EventCategory.Online),
            CreateEvent("Workshop one", BaseTime, BaseTime.AddHours(1), EventCategory.Workshop),
            CreateEvent("Workshop two", BaseTime, BaseTime.AddHours(1), EventCategory.Workshop)
        };

        var result = events
            .AsQueryable()
            .ApplyCategoryFilter(EventCategory.Workshop)
            .Select(calendarEvent => calendarEvent.Title)
            .ToArray();

        Assert.Equal(["Workshop one", "Workshop two"], result);
    }

    [Fact]
    public void ApplySearch_ShouldMatchTitleAndDescriptionIgnoringCase()
    {
        var events = new[]
        {
            CreateEvent(
                "Mentorship Panel",
                BaseTime,
                BaseTime.AddHours(1),
                description: "Career advice"
            ),
            CreateEvent(
                "Design Review",
                BaseTime,
                BaseTime.AddHours(1),
                description: "Mentor feedback"
            ),
            CreateEvent(
                "Sponsor Check-in",
                BaseTime,
                BaseTime.AddHours(1),
                description: "Booth setup"
            )
        };

        var result = events
            .AsQueryable()
            .ApplySearch("mentor")
            .Select(calendarEvent => calendarEvent.Title)
            .ToArray();

        Assert.Equal(["Mentorship Panel", "Design Review"], result);
    }

    [Fact]
    public void ApplySorting_ShouldSortByStartDateTimeAscending()
    {
        var events = new[]
        {
            CreateEvent("Second", BaseTime.AddHours(2), BaseTime.AddHours(3)),
            CreateEvent("First", BaseTime, BaseTime.AddHours(1)),
            CreateEvent("Also First", BaseTime, BaseTime.AddHours(1))
        };

        var result = events
            .AsQueryable()
            .ApplySorting(EventSortFields.StartDateTime, SortDirections.Asc)
            .Select(calendarEvent => calendarEvent.Title)
            .ToArray();

        Assert.Equal(["Also First", "First", "Second"], result);
    }

    [Fact]
    public void ApplySorting_ShouldSortByTitleDescending()
    {
        var events = new[]
        {
            CreateEvent("Alpha", BaseTime, BaseTime.AddHours(1)),
            CreateEvent("Gamma", BaseTime, BaseTime.AddHours(1)),
            CreateEvent("Beta", BaseTime, BaseTime.AddHours(1))
        };

        var result = events
            .AsQueryable()
            .ApplySorting(EventSortFields.Title, SortDirections.Desc)
            .Select(calendarEvent => calendarEvent.Title)
            .ToArray();

        Assert.Equal(["Gamma", "Beta", "Alpha"], result);
    }

    private static CalendarEvent CreateEvent(
        string title,
        DateTimeOffset startDateTime,
        DateTimeOffset endDateTime,
        EventCategory category = EventCategory.Workshop,
        string? description = null
    )
    {
        return CalendarEvent.Create(
            title,
            description,
            category,
            startDateTime,
            endDateTime,
            Guid.NewGuid(),
            BaseTime,
            "#2563EB"
        );
    }
}
