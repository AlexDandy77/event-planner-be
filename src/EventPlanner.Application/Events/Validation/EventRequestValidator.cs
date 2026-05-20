using System.Text.RegularExpressions;

using EventPlanner.Application.Common.Validation;
using EventPlanner.Application.Events.Dtos;
using EventPlanner.Application.Events.Mapping;
using EventPlanner.Domain.Entities;

namespace EventPlanner.Application.Events.Validation;

public sealed partial class EventRequestValidator
    : IRequestValidator<CreateEventRequest>,
        IRequestValidator<UpdateEventRequest>
{
    public void Validate(CreateEventRequest request)
    {
        ValidateEventFields(
            request.Title,
            request.Description,
            request.Category,
            request.StartDateTime,
            request.EndDateTime,
            request.Color
        );
    }

    public void Validate(UpdateEventRequest request)
    {
        ValidateEventFields(
            request.Title,
            request.Description,
            request.Category,
            request.StartDateTime,
            request.EndDateTime,
            request.Color
        );
    }

    private static void ValidateEventFields(
        string? title,
        string? description,
        string? category,
        DateTimeOffset startDateTime,
        DateTimeOffset endDateTime,
        string? color
    )
    {
        var errors = new ValidationErrorBag();

        ValidateTitle(title, errors);
        ValidateDescription(description, errors);
        ValidateCategory(category, errors);
        ValidateDateRange(startDateTime, endDateTime, errors);
        ValidateColor(color, errors);

        errors.ThrowIfAny();
    }

    private static void ValidateTitle(string? title, ValidationErrorBag errors)
    {
        var normalizedTitle = title?.Trim();

        if (string.IsNullOrWhiteSpace(normalizedTitle))
        {
            errors.Add("title", "Title is required.");

            return;
        }

        if (normalizedTitle.Length > CalendarEvent.MaxTitleLength)
        {
            errors.Add(
                "title",
                $"Title cannot exceed {CalendarEvent.MaxTitleLength} characters."
            );
        }
    }

    private static void ValidateDescription(string? description, ValidationErrorBag errors)
    {
        var normalizedDescription = description?.Trim();

        if (
            !string.IsNullOrEmpty(normalizedDescription)
            && normalizedDescription.Length > CalendarEvent.MaxDescriptionLength
        )
        {
            errors.Add(
                "description",
                $"Description cannot exceed {CalendarEvent.MaxDescriptionLength} characters."
            );
        }
    }

    private static void ValidateCategory(string? category, ValidationErrorBag errors)
    {
        if (string.IsNullOrWhiteSpace(category))
        {
            errors.Add("category", "Category is required.");

            return;
        }

        if (!EventCategoryMapper.IsSupportedContractValue(category))
        {
            errors.Add("category", $"Category '{category}' is not supported.");
        }
    }

    private static void ValidateDateRange(
        DateTimeOffset startDateTime,
        DateTimeOffset endDateTime,
        ValidationErrorBag errors
    )
    {
        if (startDateTime == default)
        {
            errors.Add("startDateTime", "Start date is required.");
        }

        if (endDateTime == default)
        {
            errors.Add("endDateTime", "End date is required.");
        }

        if (startDateTime != default && endDateTime != default && endDateTime <= startDateTime)
        {
            errors.Add("endDateTime", "End date must be later than start date.");
        }
    }

    private static void ValidateColor(string? color, ValidationErrorBag errors)
    {
        if (string.IsNullOrWhiteSpace(color))
        {
            errors.Add("color", "Color is required.");

            return;
        }

        if (!HexColorRegex().IsMatch(color.Trim()))
        {
            errors.Add("color", "Color must be a valid #RRGGBB hex color.");
        }
    }

    [GeneratedRegex("^#[0-9A-Fa-f]{6}$", RegexOptions.CultureInvariant)]
    private static partial Regex HexColorRegex();
}
