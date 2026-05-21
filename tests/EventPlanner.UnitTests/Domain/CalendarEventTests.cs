using EventPlanner.Domain.Common;
using EventPlanner.Domain.Entities;
using EventPlanner.Domain.Enums;

namespace EventPlanner.UnitTests.Domain;

public sealed class CalendarEventTests
{
    [Fact]
    public void Create_ShouldThrow_WhenEndDateIsNotLaterThanStartDate()
    {
        var startDateTime = DateTimeOffset.Parse("2026-05-20T09:00:00Z");

        var create = () =>
            CalendarEvent.Create(
                "Planning session",
                null,
                EventCategory.Online,
                startDateTime,
                startDateTime,
                Guid.NewGuid(),
                startDateTime,
                "#DC2626"
            );

        Assert.Throws<DomainException>(create);
    }

    [Fact]
    public void Delete_ShouldSoftDeleteEvent()
    {
        var createdAt = DateTimeOffset.Parse("2026-05-20T09:00:00Z");
        var deletedAt = createdAt.AddHours(1);

        var calendarEvent = CalendarEvent.Create(
            "Planning session",
            null,
            EventCategory.Online,
            createdAt,
            createdAt.AddHours(2),
            Guid.NewGuid(),
            createdAt,
            "#DC2626"
        );

        calendarEvent.Delete(deletedAt);

        Assert.True(calendarEvent.IsDeleted);
        Assert.Equal(deletedAt, calendarEvent.UpdatedAt);
    }
}
