using EventPlanner.Domain.Entities;
using EventPlanner.Domain.Enums;

using Microsoft.EntityFrameworkCore;

namespace EventPlanner.Infrastructure.Persistence.Seed;

internal static class DevelopmentSeedData
{
    private static readonly Guid OrganizerId = Guid.Parse("8a5edca7-8f80-4ed6-8b91-01863782b51b");
    private static readonly Guid AdminId = Guid.Parse("f1a70547-12ce-4d4f-b5b1-a22cb6efdf5d");
    private static readonly Guid CommunityOrganizerId = Guid.Parse(
        "c7d731cc-7e02-4d89-9330-4210e4221ab5"
    );
    private static readonly Guid DesignOrganizerId = Guid.Parse(
        "a9cb5e4e-7777-4e5d-bef3-6d795663b246"
    );
    private static readonly Guid OperationsOrganizerId = Guid.Parse(
        "77ee5d58-ef01-449d-a418-936c19d0cfdf"
    );

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
            },
            new
            {
                Id = AdminId,
                FirstName = "Alex",
                LastName = "Admin",
                Email = "admin@example.com",
                Role = UserRole.Admin,
                Phone = "+37360000001",
                PasswordHash = "development-password-hash",
                IsActive = true,
                CreatedAt = SeededAt,
                UpdatedAt = SeededAt
            },
            new
            {
                Id = CommunityOrganizerId,
                FirstName = "Mara",
                LastName = "Community",
                Email = "mara.community@example.com",
                Role = UserRole.Organizer,
                Phone = "+37360000002",
                PasswordHash = "development-password-hash",
                IsActive = true,
                CreatedAt = SeededAt,
                UpdatedAt = SeededAt
            },
            new
            {
                Id = DesignOrganizerId,
                FirstName = "Victor",
                LastName = "Design",
                Email = "victor.design@example.com",
                Role = UserRole.Organizer,
                Phone = "+37360000003",
                PasswordHash = "development-password-hash",
                IsActive = true,
                CreatedAt = SeededAt,
                UpdatedAt = SeededAt
            },
            new
            {
                Id = OperationsOrganizerId,
                FirstName = "Nina",
                LastName = "Operations",
                Email = "nina.operations@example.com",
                Role = UserRole.Organizer,
                Phone = "+37360000004",
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
            },
            new
            {
                Id = Guid.Parse("2d4487e7-8403-4454-b0f2-e5d5201e42e6"),
                Title = "Frontend Calendar Workshop",
                Description = "Hands-on session for building calendar interactions.",
                Category = EventCategory.Workshop,
                StartDateTime = new DateTimeOffset(2026, 5, 22, 10, 0, 0, TimeSpan.Zero),
                EndDateTime = new DateTimeOffset(2026, 5, 22, 12, 0, 0, TimeSpan.Zero),
                OrganizerId = DesignOrganizerId,
                CreatedAt = SeededAt,
                UpdatedAt = SeededAt,
                Color = "#7C3AED",
                IsDeleted = false
            },
            new
            {
                Id = Guid.Parse("e5947a8d-4730-4210-8daa-90058b05fef2"),
                Title = "Community Networking Evening",
                Description = "Informal meetup for students, mentors, and organizers.",
                Category = EventCategory.InPerson,
                StartDateTime = new DateTimeOffset(2026, 5, 22, 17, 30, 0, TimeSpan.Zero),
                EndDateTime = new DateTimeOffset(2026, 5, 22, 19, 0, 0, TimeSpan.Zero),
                OrganizerId = CommunityOrganizerId,
                CreatedAt = SeededAt,
                UpdatedAt = SeededAt,
                Color = "#F97316",
                IsDeleted = false
            },
            new
            {
                Id = Guid.Parse("3dfc25b7-c221-40ef-afc3-a8795ee6513f"),
                Title = "Venue Walkthrough",
                Description = "Check rooms, signage, registration desk, and equipment.",
                Category = EventCategory.InPerson,
                StartDateTime = new DateTimeOffset(2026, 5, 23, 8, 30, 0, TimeSpan.Zero),
                EndDateTime = new DateTimeOffset(2026, 5, 23, 10, 0, 0, TimeSpan.Zero),
                OrganizerId = OperationsOrganizerId,
                CreatedAt = SeededAt,
                UpdatedAt = SeededAt,
                Color = "#059669",
                IsDeleted = false
            },
            new
            {
                Id = Guid.Parse("f29d4f5d-3972-4e41-b2a5-405137fb44dc"),
                Title = "Sponsor Check-in",
                Description = "Review sponsor booth needs and presentation materials.",
                Category = EventCategory.Online,
                StartDateTime = new DateTimeOffset(2026, 5, 23, 13, 0, 0, TimeSpan.Zero),
                EndDateTime = new DateTimeOffset(2026, 5, 23, 13, 45, 0, TimeSpan.Zero),
                OrganizerId = OrganizerId,
                CreatedAt = SeededAt,
                UpdatedAt = SeededAt,
                Color = "#0891B2",
                IsDeleted = false
            },
            new
            {
                Id = Guid.Parse("57c4877d-b218-42e7-8040-c6f422f2641d"),
                Title = "Hybrid Mentorship Panel",
                Description = "Panel with mentors joining both onsite and remotely.",
                Category = EventCategory.Hybrid,
                StartDateTime = new DateTimeOffset(2026, 5, 24, 11, 0, 0, TimeSpan.Zero),
                EndDateTime = new DateTimeOffset(2026, 5, 24, 12, 30, 0, TimeSpan.Zero),
                OrganizerId = CommunityOrganizerId,
                CreatedAt = SeededAt,
                UpdatedAt = SeededAt,
                Color = "#DB2777",
                IsDeleted = false
            },
            new
            {
                Id = Guid.Parse("b605d60d-67c9-4497-b53a-e69f0d48c044"),
                Title = "Conference Opening Keynote",
                Description = "Kickoff keynote for the event planner internship demo.",
                Category = EventCategory.Conference,
                StartDateTime = new DateTimeOffset(2026, 5, 25, 9, 0, 0, TimeSpan.Zero),
                EndDateTime = new DateTimeOffset(2026, 5, 25, 10, 0, 0, TimeSpan.Zero),
                OrganizerId = OrganizerId,
                CreatedAt = SeededAt,
                UpdatedAt = SeededAt,
                Color = "#DC2626",
                IsDeleted = false
            },
            new
            {
                Id = Guid.Parse("7dc6bdcb-463d-442d-a976-86a673e5cebf"),
                Title = "Volunteer Training",
                Description = "Prepare volunteers for registration and attendee support.",
                Category = EventCategory.Workshop,
                StartDateTime = new DateTimeOffset(2026, 5, 25, 14, 0, 0, TimeSpan.Zero),
                EndDateTime = new DateTimeOffset(2026, 5, 25, 15, 30, 0, TimeSpan.Zero),
                OrganizerId = OperationsOrganizerId,
                CreatedAt = SeededAt,
                UpdatedAt = SeededAt,
                Color = "#16A34A",
                IsDeleted = false
            },
            new
            {
                Id = Guid.Parse("1d7ef292-5650-4534-a78c-f6076dd726be"),
                Title = "Design Review",
                Description = "Review event branding, badges, and visual materials.",
                Category = EventCategory.Other,
                StartDateTime = new DateTimeOffset(2026, 5, 26, 10, 30, 0, TimeSpan.Zero),
                EndDateTime = new DateTimeOffset(2026, 5, 26, 11, 30, 0, TimeSpan.Zero),
                OrganizerId = DesignOrganizerId,
                CreatedAt = SeededAt,
                UpdatedAt = SeededAt,
                Color = "#9333EA",
                IsDeleted = false
            },
            new
            {
                Id = Guid.Parse("7aefc1e5-9c7a-4df4-9d55-b7a5197d1b96"),
                Title = "Admin Operations Review",
                Description = "Admin review of event readiness, risks, and next actions.",
                Category = EventCategory.Online,
                StartDateTime = new DateTimeOffset(2026, 5, 27, 16, 0, 0, TimeSpan.Zero),
                EndDateTime = new DateTimeOffset(2026, 5, 27, 17, 0, 0, TimeSpan.Zero),
                OrganizerId = AdminId,
                CreatedAt = SeededAt,
                UpdatedAt = SeededAt,
                Color = "#0F172A",
                IsDeleted = false
            },
            new
            {
                Id = Guid.Parse("5d487840-184d-4ce5-875a-56dadbe446a3"),
                Title = "Post-event Retrospective",
                Description = "Collect lessons learned and improvement ideas.",
                Category = EventCategory.Hybrid,
                StartDateTime = new DateTimeOffset(2026, 5, 29, 15, 0, 0, TimeSpan.Zero),
                EndDateTime = new DateTimeOffset(2026, 5, 29, 16, 30, 0, TimeSpan.Zero),
                OrganizerId = OrganizerId,
                CreatedAt = SeededAt,
                UpdatedAt = SeededAt,
                Color = "#EA580C",
                IsDeleted = false
            }
        );
    }
}
