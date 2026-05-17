using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EventPlanner.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDevelopmentSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "calendar_events",
                columns: new[] { "Id", "Category", "Color", "CreatedAt", "Description", "EndDateTime", "OrganizerId", "StartDateTime", "Title", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("5d487840-184d-4ce5-875a-56dadbe446a3"), "Hybrid", "#EA580C", new DateTimeOffset(new DateTime(2026, 5, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Collect lessons learned and improvement ideas.", new DateTimeOffset(new DateTime(2026, 5, 29, 16, 30, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("8a5edca7-8f80-4ed6-8b91-01863782b51b"), new DateTimeOffset(new DateTime(2026, 5, 29, 15, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Post-event Retrospective", new DateTimeOffset(new DateTime(2026, 5, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("b605d60d-67c9-4497-b53a-e69f0d48c044"), "Conference", "#DC2626", new DateTimeOffset(new DateTime(2026, 5, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Kickoff keynote for the event planner internship demo.", new DateTimeOffset(new DateTime(2026, 5, 25, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("8a5edca7-8f80-4ed6-8b91-01863782b51b"), new DateTimeOffset(new DateTime(2026, 5, 25, 9, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Conference Opening Keynote", new DateTimeOffset(new DateTime(2026, 5, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("f29d4f5d-3972-4e41-b2a5-405137fb44dc"), "Online", "#0891B2", new DateTimeOffset(new DateTime(2026, 5, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Review sponsor booth needs and presentation materials.", new DateTimeOffset(new DateTime(2026, 5, 23, 13, 45, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("8a5edca7-8f80-4ed6-8b91-01863782b51b"), new DateTimeOffset(new DateTime(2026, 5, 23, 13, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Sponsor Check-in", new DateTimeOffset(new DateTime(2026, 5, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) }
                });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "Id", "CreatedAt", "Email", "FirstName", "IsActive", "LastName", "PasswordHash", "Phone", "Role", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("77ee5d58-ef01-449d-a418-936c19d0cfdf"), new DateTimeOffset(new DateTime(2026, 5, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "nina.operations@example.com", "Nina", true, "Operations", "development-password-hash", "+37360000004", "Organizer", new DateTimeOffset(new DateTime(2026, 5, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("a9cb5e4e-7777-4e5d-bef3-6d795663b246"), new DateTimeOffset(new DateTime(2026, 5, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "victor.design@example.com", "Victor", true, "Design", "development-password-hash", "+37360000003", "Organizer", new DateTimeOffset(new DateTime(2026, 5, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("c7d731cc-7e02-4d89-9330-4210e4221ab5"), new DateTimeOffset(new DateTime(2026, 5, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "mara.community@example.com", "Mara", true, "Community", "development-password-hash", "+37360000002", "Organizer", new DateTimeOffset(new DateTime(2026, 5, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("f1a70547-12ce-4d4f-b5b1-a22cb6efdf5d"), new DateTimeOffset(new DateTime(2026, 5, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "admin@example.com", "Alex", true, "Admin", "development-password-hash", "+37360000001", "Admin", new DateTimeOffset(new DateTime(2026, 5, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) }
                });

            migrationBuilder.InsertData(
                table: "calendar_events",
                columns: new[] { "Id", "Category", "Color", "CreatedAt", "Description", "EndDateTime", "OrganizerId", "StartDateTime", "Title", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("1d7ef292-5650-4534-a78c-f6076dd726be"), "Other", "#9333EA", new DateTimeOffset(new DateTime(2026, 5, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Review event branding, badges, and visual materials.", new DateTimeOffset(new DateTime(2026, 5, 26, 11, 30, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("a9cb5e4e-7777-4e5d-bef3-6d795663b246"), new DateTimeOffset(new DateTime(2026, 5, 26, 10, 30, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Design Review", new DateTimeOffset(new DateTime(2026, 5, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("2d4487e7-8403-4454-b0f2-e5d5201e42e6"), "Workshop", "#7C3AED", new DateTimeOffset(new DateTime(2026, 5, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Hands-on session for building calendar interactions.", new DateTimeOffset(new DateTime(2026, 5, 22, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("a9cb5e4e-7777-4e5d-bef3-6d795663b246"), new DateTimeOffset(new DateTime(2026, 5, 22, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Frontend Calendar Workshop", new DateTimeOffset(new DateTime(2026, 5, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("3dfc25b7-c221-40ef-afc3-a8795ee6513f"), "InPerson", "#059669", new DateTimeOffset(new DateTime(2026, 5, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Check rooms, signage, registration desk, and equipment.", new DateTimeOffset(new DateTime(2026, 5, 23, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("77ee5d58-ef01-449d-a418-936c19d0cfdf"), new DateTimeOffset(new DateTime(2026, 5, 23, 8, 30, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Venue Walkthrough", new DateTimeOffset(new DateTime(2026, 5, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("57c4877d-b218-42e7-8040-c6f422f2641d"), "Hybrid", "#DB2777", new DateTimeOffset(new DateTime(2026, 5, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Panel with mentors joining both onsite and remotely.", new DateTimeOffset(new DateTime(2026, 5, 24, 12, 30, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("c7d731cc-7e02-4d89-9330-4210e4221ab5"), new DateTimeOffset(new DateTime(2026, 5, 24, 11, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Hybrid Mentorship Panel", new DateTimeOffset(new DateTime(2026, 5, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("7aefc1e5-9c7a-4df4-9d55-b7a5197d1b96"), "Online", "#0F172A", new DateTimeOffset(new DateTime(2026, 5, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Admin review of event readiness, risks, and next actions.", new DateTimeOffset(new DateTime(2026, 5, 27, 17, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("f1a70547-12ce-4d4f-b5b1-a22cb6efdf5d"), new DateTimeOffset(new DateTime(2026, 5, 27, 16, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Admin Operations Review", new DateTimeOffset(new DateTime(2026, 5, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("7dc6bdcb-463d-442d-a976-86a673e5cebf"), "Workshop", "#16A34A", new DateTimeOffset(new DateTime(2026, 5, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Prepare volunteers for registration and attendee support.", new DateTimeOffset(new DateTime(2026, 5, 25, 15, 30, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("77ee5d58-ef01-449d-a418-936c19d0cfdf"), new DateTimeOffset(new DateTime(2026, 5, 25, 14, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Volunteer Training", new DateTimeOffset(new DateTime(2026, 5, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) },
                    { new Guid("e5947a8d-4730-4210-8daa-90058b05fef2"), "InPerson", "#F97316", new DateTimeOffset(new DateTime(2026, 5, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Informal meetup for students, mentors, and organizers.", new DateTimeOffset(new DateTime(2026, 5, 22, 19, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("c7d731cc-7e02-4d89-9330-4210e4221ab5"), new DateTimeOffset(new DateTime(2026, 5, 22, 17, 30, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "Community Networking Evening", new DateTimeOffset(new DateTime(2026, 5, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "calendar_events",
                keyColumn: "Id",
                keyValue: new Guid("1d7ef292-5650-4534-a78c-f6076dd726be"));

            migrationBuilder.DeleteData(
                table: "calendar_events",
                keyColumn: "Id",
                keyValue: new Guid("2d4487e7-8403-4454-b0f2-e5d5201e42e6"));

            migrationBuilder.DeleteData(
                table: "calendar_events",
                keyColumn: "Id",
                keyValue: new Guid("3dfc25b7-c221-40ef-afc3-a8795ee6513f"));

            migrationBuilder.DeleteData(
                table: "calendar_events",
                keyColumn: "Id",
                keyValue: new Guid("57c4877d-b218-42e7-8040-c6f422f2641d"));

            migrationBuilder.DeleteData(
                table: "calendar_events",
                keyColumn: "Id",
                keyValue: new Guid("5d487840-184d-4ce5-875a-56dadbe446a3"));

            migrationBuilder.DeleteData(
                table: "calendar_events",
                keyColumn: "Id",
                keyValue: new Guid("7aefc1e5-9c7a-4df4-9d55-b7a5197d1b96"));

            migrationBuilder.DeleteData(
                table: "calendar_events",
                keyColumn: "Id",
                keyValue: new Guid("7dc6bdcb-463d-442d-a976-86a673e5cebf"));

            migrationBuilder.DeleteData(
                table: "calendar_events",
                keyColumn: "Id",
                keyValue: new Guid("b605d60d-67c9-4497-b53a-e69f0d48c044"));

            migrationBuilder.DeleteData(
                table: "calendar_events",
                keyColumn: "Id",
                keyValue: new Guid("e5947a8d-4730-4210-8daa-90058b05fef2"));

            migrationBuilder.DeleteData(
                table: "calendar_events",
                keyColumn: "Id",
                keyValue: new Guid("f29d4f5d-3972-4e41-b2a5-405137fb44dc"));

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("77ee5d58-ef01-449d-a418-936c19d0cfdf"));

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("a9cb5e4e-7777-4e5d-bef3-6d795663b246"));

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("c7d731cc-7e02-4d89-9330-4210e4221ab5"));

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "Id",
                keyValue: new Guid("f1a70547-12ce-4d4f-b5b1-a22cb6efdf5d"));
        }
    }
}
