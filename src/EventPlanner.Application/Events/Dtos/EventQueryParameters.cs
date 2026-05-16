using EventPlanner.Application.Common.Dtos;

namespace EventPlanner.Application.Events.Dtos;

public sealed record EventQueryParameters
{
    public DateTimeOffset? From { get; init; }

    public DateTimeOffset? To { get; init; }

    public string? Category { get; init; }

    public string? Search { get; init; }

    public string SortBy { get; init; } = EventSortFields.StartDateTime;

    public string SortDirection { get; init; } = SortDirections.Asc;
}
