using EventPlanner.Domain.Enums;

namespace EventPlanner.Application.Events.Queries;

public sealed record EventQuery(
    DateTimeOffset? From,
    DateTimeOffset? To,
    EventCategory? Category,
    string? Search,
    string SortBy,
    string SortDirection
);
