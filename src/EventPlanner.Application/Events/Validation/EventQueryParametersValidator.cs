using EventPlanner.Application.Common.Dtos;
using EventPlanner.Application.Common.Validation;
using EventPlanner.Application.Events.Dtos;
using EventPlanner.Application.Events.Mapping;

namespace EventPlanner.Application.Events.Validation;

public sealed class EventQueryParametersValidator : IRequestValidator<EventQueryParameters>
{
    public void Validate(EventQueryParameters request)
    {
        var errors = new ValidationErrorBag();

        if (request.From.HasValue && request.To.HasValue && request.From >= request.To)
        {
            errors.Add("from", "From must be earlier than to.");
            errors.Add("to", "To must be later than from.");
        }

        if (
            !string.IsNullOrWhiteSpace(request.Category)
            && !EventCategoryMapper.IsSupportedContractValue(request.Category)
        )
        {
            errors.Add("category", $"Category '{request.Category}' is not supported.");
        }

        if (!IsSupportedSortField(request.SortBy))
        {
            errors.Add("sortBy", $"Sort field '{request.SortBy}' is not supported.");
        }

        if (!IsSupportedSortDirection(request.SortDirection))
        {
            errors.Add(
                "sortDirection",
                $"Sort direction '{request.SortDirection}' is not supported."
            );
        }

        errors.ThrowIfAny();
    }

    private static bool IsSupportedSortField(string? sortBy)
    {
        return string.IsNullOrWhiteSpace(sortBy)
            || sortBy.Equals(EventSortFields.StartDateTime, StringComparison.OrdinalIgnoreCase)
            || sortBy.Equals(EventSortFields.Title, StringComparison.OrdinalIgnoreCase)
            || sortBy.Equals(EventSortFields.CreatedAt, StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsSupportedSortDirection(string? sortDirection)
    {
        return string.IsNullOrWhiteSpace(sortDirection)
            || sortDirection.Equals(SortDirections.Asc, StringComparison.OrdinalIgnoreCase)
            || sortDirection.Equals(SortDirections.Desc, StringComparison.OrdinalIgnoreCase);
    }
}
