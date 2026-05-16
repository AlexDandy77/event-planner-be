using EventPlanner.Domain.Entities;
using EventPlanner.Domain.Enums;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventPlanner.Infrastructure.Persistence.Configurations;

public sealed class CalendarEventConfiguration : IEntityTypeConfiguration<CalendarEvent>
{
    public void Configure(EntityTypeBuilder<CalendarEvent> builder)
    {
        builder.ToTable("calendar_events");

        builder.HasKey(calendarEvent => calendarEvent.Id);

        builder.Property(calendarEvent => calendarEvent.Id).ValueGeneratedNever();

        builder
            .Property(calendarEvent => calendarEvent.Title)
            .HasMaxLength(CalendarEvent.MaxTitleLength)
            .IsRequired();

        builder
            .Property(calendarEvent => calendarEvent.Description)
            .HasMaxLength(CalendarEvent.MaxDescriptionLength);

        builder
            .Property(calendarEvent => calendarEvent.Category)
            .HasConversion(
                category => category.ToString(),
                value => Enum.Parse<EventCategory>(value)
            )
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(calendarEvent => calendarEvent.StartDateTime).IsRequired();

        builder.Property(calendarEvent => calendarEvent.EndDateTime).IsRequired();

        builder.Property(calendarEvent => calendarEvent.OrganizerId).IsRequired();

        builder.Property(calendarEvent => calendarEvent.CreatedAt).IsRequired();

        builder.Property(calendarEvent => calendarEvent.UpdatedAt).IsRequired();

        builder
            .Property(calendarEvent => calendarEvent.Color)
            .HasMaxLength(7)
            .IsFixedLength()
            .IsRequired();

        builder.Property(calendarEvent => calendarEvent.IsDeleted).HasDefaultValue(false);

        builder.HasQueryFilter(calendarEvent => !calendarEvent.IsDeleted);

        builder
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(calendarEvent => calendarEvent.OrganizerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(calendarEvent => calendarEvent.StartDateTime);
        builder.HasIndex(calendarEvent => calendarEvent.OrganizerId);
        builder.HasIndex(calendarEvent => calendarEvent.IsDeleted);
    }
}
