using EventPlanner.Application.Common.Exceptions;
using EventPlanner.Application.Events.Dtos;
using EventPlanner.Application.Events.Validation;

namespace EventPlanner.UnitTests.Application;

public sealed class EventRequestValidatorTests
{
    private static readonly DateTimeOffset FixedNow = DateTimeOffset.Parse(
        "2026-05-21T12:00:00Z"
    );

    [Fact]
    public void CreateEventRequestValidator_ShouldReturnFieldErrors_WhenRequestIsInvalid()
    {
        var validator = new EventRequestValidator();

        var validate = () =>
            validator.Validate(
                new CreateEventRequest
                {
                    Title = " ",
                    Description = new string('D', 1001),
                    Category = "party",
                    StartDateTime = FixedNow,
                    EndDateTime = FixedNow,
                    Color = "red"
                }
            );

        var exception = Assert.Throws<ApplicationValidationException>(validate);

        Assert.Contains("title", exception.Errors.Keys);
        Assert.Contains("description", exception.Errors.Keys);
        Assert.Contains("category", exception.Errors.Keys);
        Assert.Contains("endDateTime", exception.Errors.Keys);
        Assert.Contains("color", exception.Errors.Keys);
    }

    [Fact]
    public void UpdateEventRequestValidator_ShouldNotThrow_WhenRequestIsValid()
    {
        var validator = new EventRequestValidator();

        validator.Validate(
            new UpdateEventRequest
            {
                Title = "Planning Session",
                Description = "Discuss the roadmap.",
                Category = "online",
                StartDateTime = FixedNow,
                EndDateTime = FixedNow.AddHours(1),
                Color = "#DC2626"
            }
        );
    }

    [Fact]
    public void EventQueryParametersValidator_ShouldReturnFieldErrors_WhenRequestIsInvalid()
    {
        var validator = new EventQueryParametersValidator();

        var validate = () =>
            validator.Validate(
                new EventQueryParameters
                {
                    From = FixedNow,
                    To = FixedNow.AddHours(-1),
                    Category = "party",
                    SortBy = "priority",
                    SortDirection = "sideways"
                }
            );

        var exception = Assert.Throws<ApplicationValidationException>(validate);

        Assert.Contains("from", exception.Errors.Keys);
        Assert.Contains("to", exception.Errors.Keys);
        Assert.Contains("category", exception.Errors.Keys);
        Assert.Contains("sortBy", exception.Errors.Keys);
        Assert.Contains("sortDirection", exception.Errors.Keys);
    }
}
