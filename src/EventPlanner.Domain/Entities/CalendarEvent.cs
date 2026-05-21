using System.Text.RegularExpressions;

using EventPlanner.Domain.Common;
using EventPlanner.Domain.Enums;

namespace EventPlanner.Domain.Entities;

public sealed class CalendarEvent
{
    public const int MaxTitleLength = 120;
    public const int MaxDescriptionLength = 1000;

    private static readonly Regex HexColorRegex = new(
        "^#[0-9A-Fa-f]{6}$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant
    );

    private CalendarEvent()
    {
    }

    private CalendarEvent(
        Guid id,
        string title,
        string? description,
        EventCategory category,
        DateTimeOffset startDateTime,
        DateTimeOffset endDateTime,
        Guid organizerId,
        DateTimeOffset createdAt,
        string color
    )
    {
        Id = id;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
        OrganizerId = organizerId;
        ApplyDetails(title, description, category, startDateTime, endDateTime, color);
    }

    public Guid Id { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public EventCategory Category { get; private set; }

    public DateTimeOffset StartDateTime { get; private set; }

    public DateTimeOffset EndDateTime { get; private set; }

    public Guid OrganizerId { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public string Color { get; private set; } = string.Empty;

    public bool IsDeleted { get; private set; }

    public static CalendarEvent Create(
        string title,
        string? description,
        EventCategory category,
        DateTimeOffset startDateTime,
        DateTimeOffset endDateTime,
        Guid organizerId,
        DateTimeOffset createdAt,
        string color
    )
    {
        if (organizerId == Guid.Empty)
        {
            throw new DomainException("Organizer id is required.");
        }

        return new CalendarEvent(
            Guid.NewGuid(),
            title,
            description,
            category,
            startDateTime,
            endDateTime,
            organizerId,
            createdAt,
            color
        );
    }

    public void UpdateDetails(
        string title,
        string? description,
        EventCategory category,
        DateTimeOffset startDateTime,
        DateTimeOffset endDateTime,
        string color,
        DateTimeOffset updatedAt
    )
    {
        EnsureNotDeleted();
        ApplyDetails(title, description, category, startDateTime, endDateTime, color);
        UpdatedAt = updatedAt;
    }

    public void Delete(DateTimeOffset deletedAt)
    {
        EnsureNotDeleted();
        IsDeleted = true;
        UpdatedAt = deletedAt;
    }

    private void ApplyDetails(
        string title,
        string? description,
        EventCategory category,
        DateTimeOffset startDateTime,
        DateTimeOffset endDateTime,
        string color
    )
    {
        Title = NormalizeTitle(title);
        Description = NormalizeDescription(description);
        Category = category;
        StartDateTime = startDateTime;
        EndDateTime = EnsureEndDateTime(startDateTime, endDateTime);
        Color = NormalizeColor(color);
    }

    private static string NormalizeTitle(string title)
    {
        var normalizedTitle = title.Trim();

        if (string.IsNullOrWhiteSpace(normalizedTitle))
        {
            throw new DomainException("Event title is required.");
        }

        if (normalizedTitle.Length > MaxTitleLength)
        {
            throw new DomainException($"Event title cannot exceed {MaxTitleLength} characters.");
        }

        return normalizedTitle;
    }

    private static string? NormalizeDescription(string? description)
    {
        var normalizedDescription = description?.Trim();

        if (string.IsNullOrEmpty(normalizedDescription))
        {
            return null;
        }

        if (normalizedDescription.Length > MaxDescriptionLength)
        {
            throw new DomainException(
                $"Event description cannot exceed {MaxDescriptionLength} characters."
            );
        }

        return normalizedDescription;
    }

    private static DateTimeOffset EnsureEndDateTime(
        DateTimeOffset startDateTime,
        DateTimeOffset endDateTime
    )
    {
        if (endDateTime <= startDateTime)
        {
            throw new DomainException("Event end date must be later than start date.");
        }

        return endDateTime;
    }

    private static string NormalizeColor(string color)
    {
        var normalizedColor = color.Trim();

        if (!HexColorRegex.IsMatch(normalizedColor))
        {
            throw new DomainException("Event color must be a valid #RRGGBB hex color.");
        }

        return normalizedColor.ToUpperInvariant();
    }

    private void EnsureNotDeleted()
    {
        if (IsDeleted)
        {
            throw new DomainException("Deleted events cannot be changed.");
        }
    }
}
