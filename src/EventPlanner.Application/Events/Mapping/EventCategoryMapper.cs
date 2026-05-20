using EventPlanner.Application.Common.Exceptions;
using EventPlanner.Domain.Enums;

namespace EventPlanner.Application.Events.Mapping;

internal static class EventCategoryMapper
{
    public static string ToContractValue(EventCategory category)
    {
        return category switch
        {
            EventCategory.Online => "online",
            EventCategory.InPerson => "inPerson",
            EventCategory.Hybrid => "hybrid",
            EventCategory.Workshop => "workshop",
            EventCategory.Conference => "conference",
            EventCategory.Other => "other",
            _ => throw new ApplicationValidationException("Unsupported event category.")
        };
    }

    public static EventCategory ToDomainValue(string? category)
    {
        if (string.IsNullOrWhiteSpace(category))
        {
            throw new ApplicationValidationException("Event category is required.");
        }

        var normalizedCategory = category.Trim();

        if (normalizedCategory.Equals("online", StringComparison.OrdinalIgnoreCase))
        {
            return EventCategory.Online;
        }

        if (normalizedCategory.Equals("inPerson", StringComparison.OrdinalIgnoreCase))
        {
            return EventCategory.InPerson;
        }

        if (normalizedCategory.Equals("hybrid", StringComparison.OrdinalIgnoreCase))
        {
            return EventCategory.Hybrid;
        }

        if (normalizedCategory.Equals("workshop", StringComparison.OrdinalIgnoreCase))
        {
            return EventCategory.Workshop;
        }

        if (normalizedCategory.Equals("conference", StringComparison.OrdinalIgnoreCase))
        {
            return EventCategory.Conference;
        }

        if (normalizedCategory.Equals("other", StringComparison.OrdinalIgnoreCase))
        {
            return EventCategory.Other;
        }

        throw new ApplicationValidationException(
            $"Event category '{category}' is not supported."
        );
    }

    public static bool IsSupportedContractValue(string? category)
    {
        if (string.IsNullOrWhiteSpace(category))
        {
            return false;
        }

        var normalizedCategory = category.Trim();

        return normalizedCategory.Equals("online", StringComparison.OrdinalIgnoreCase)
            || normalizedCategory.Equals("inPerson", StringComparison.OrdinalIgnoreCase)
            || normalizedCategory.Equals("hybrid", StringComparison.OrdinalIgnoreCase)
            || normalizedCategory.Equals("workshop", StringComparison.OrdinalIgnoreCase)
            || normalizedCategory.Equals("conference", StringComparison.OrdinalIgnoreCase)
            || normalizedCategory.Equals("other", StringComparison.OrdinalIgnoreCase);
    }
}
