namespace EventPlanner.Application.Events.Dtos;

public sealed record UpdateEventRequest
{
    public string Title { get; init; } = string.Empty;

    public string? Description { get; init; }

    public string Category { get; init; } = string.Empty;

    public DateTimeOffset StartDateTime { get; init; }

    public DateTimeOffset EndDateTime { get; init; }

    public string Color { get; init; } = string.Empty;
}
