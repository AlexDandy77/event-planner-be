using EventPlanner.Domain.Entities;
using EventPlanner.Infrastructure.Persistence.Seed;

using Microsoft.EntityFrameworkCore;

namespace EventPlanner.Infrastructure.Persistence;

public sealed class EventPlannerDbContext(DbContextOptions<EventPlannerDbContext> options) : DbContext(options)
{
    public DbSet<CalendarEvent> CalendarEvents => Set<CalendarEvent>();

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EventPlannerDbContext).Assembly);
        modelBuilder.SeedDevelopmentData();
    }
}
