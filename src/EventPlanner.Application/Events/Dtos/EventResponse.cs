namespace EventPlanner.Application.Events.Dtos;

public sealed record EventResponse(
    Guid Id,
    string Title,
    string? Description,
    string Category,
    DateTimeOffset StartDateTime,
    DateTimeOffset EndDateTime,
    Guid OrganizerId,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    string Color,
    bool IsDeleted
);
