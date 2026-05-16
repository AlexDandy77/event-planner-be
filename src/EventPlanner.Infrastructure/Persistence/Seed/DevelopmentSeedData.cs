using EventPlanner.Domain.Entities;
using EventPlanner.Domain.Enums;

using Microsoft.EntityFrameworkCore;

namespace EventPlanner.Infrastructure.Persistence.Seed;

internal static class DevelopmentSeedData
{
    private static readonly Guid OrganizerId = Guid.Parse("8a5edca7-8f80-4ed6-8b91-01863782b51b");

    private static readonly DateTimeOffset SeededAt = new(
        2026,
        5,
        1,
        10,
        0,
        0,
        TimeSpan.Zero
    );

    public static void SeedDevelopmentData(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasData(
            new
            {
                Id = OrganizerId,
                FirstName = "Demo",
                LastName = "Organizer",
                Email = "organizer@example.com",
                Role = UserRole.Organizer,
                Phone = "+37360000000",
                PasswordHash = "development-password-hash",
                IsActive = true,
                CreatedAt = SeededAt,
                UpdatedAt = SeededAt
            }
        );

        modelBuilder.Entity<CalendarEvent>().HasData(
            new
            {
                Id = Guid.Parse("4c516b2a-7f1f-4c19-9a0d-09362d3d2fd1"),
                Title = "Tech Career Fair 2026",
                Description = "An event for IT students and companies.",
                Category = EventCategory.Online,
                StartDateTime = new DateTimeOffset(2026, 5, 20, 9, 0, 0, TimeSpan.Zero),
                EndDateTime = new DateTimeOffset(2026, 5, 20, 11, 0, 0, TimeSpan.Zero),
                OrganizerId,
                CreatedAt = SeededAt,
                UpdatedAt = SeededAt,
                Color = "#DC2626",
                IsDeleted = false
            },
            new
            {
                Id = Guid.Parse("d8204401-a77d-44d4-ae96-98e31f59a580"),
                Title = "Product Planning",
                Description = "Roadmap discussion for the event platform.",
                Category = EventCategory.Workshop,
                StartDateTime = new DateTimeOffset(2026, 5, 21, 13, 0, 0, TimeSpan.Zero),
                EndDateTime = new DateTimeOffset(2026, 5, 21, 14, 30, 0, TimeSpan.Zero),
                OrganizerId,
                CreatedAt = SeededAt,
                UpdatedAt = SeededAt,
                Color = "#2563EB",
                IsDeleted = false
            }
        );
    }
}
