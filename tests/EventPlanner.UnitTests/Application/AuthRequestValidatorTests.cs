using EventPlanner.Application.Auth.Dtos;
using EventPlanner.Application.Auth.Validation;
using EventPlanner.Application.Common.Exceptions;

namespace EventPlanner.UnitTests.Application;

public sealed class AuthRequestValidatorTests
{
    [Fact]
    public void RegisterRequestValidator_ShouldReturnFieldErrors_WhenRequestIsInvalid()
    {
        var validator = new RegisterRequestValidator();

        var validate = () =>
            validator.Validate(
                new RegisterRequest
                {
                    FirstName = " ",
                    LastName = new string('L', 81),
                    Email = "not-an-email",
                    Password = "short",
                    Phone = new string('1', 31)
                }
            );

        var exception = Assert.Throws<ApplicationValidationException>(validate);

        Assert.Contains("firstName", exception.Errors.Keys);
        Assert.Contains("lastName", exception.Errors.Keys);
        Assert.Contains("email", exception.Errors.Keys);
        Assert.Contains("password", exception.Errors.Keys);
        Assert.Contains("phone", exception.Errors.Keys);
    }

    [Fact]
    public void LoginRequestValidator_ShouldReturnFieldErrors_WhenRequestIsInvalid()
    {
        var validator = new LoginRequestValidator();

        var validate = () =>
            validator.Validate(
                new LoginRequest
                {
                    Email = "invalid-email",
                    Password = " "
                }
            );

        var exception = Assert.Throws<ApplicationValidationException>(validate);

        Assert.Contains("email", exception.Errors.Keys);
        Assert.Contains("password", exception.Errors.Keys);
    }
}
