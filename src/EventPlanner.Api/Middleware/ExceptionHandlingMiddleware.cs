using System.Text.Json;

using EventPlanner.Application.Common.Exceptions;
using EventPlanner.Domain.Common;

using Microsoft.AspNetCore.Mvc;

namespace EventPlanner.Api.Middleware;

public sealed class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger
    )
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly ILogger<ExceptionHandlingMiddleware> _logger = logger;
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        if (exception is ApplicationValidationException validationException)
        {
            _logger.LogWarning("Request failed with validation errors.");

            await WriteValidationProblemAsync(context, validationException);

            return;
        }

        var (statusCode, title, detail, shouldLogAsError) = exception switch
        {
            ApplicationAuthenticationException => (
                StatusCodes.Status401Unauthorized,
                "Unauthorized",
                exception.Message,
                false
            ),
            ApplicationForbiddenException => (
                StatusCodes.Status403Forbidden,
                "Forbidden",
                exception.Message,
                false
            ),
            ApplicationConflictException => (
                StatusCodes.Status409Conflict,
                "Conflict",
                exception.Message,
                false
            ),
            DomainException => (
                StatusCodes.Status400BadRequest,
                "Bad Request",
                exception.Message,
                false
            ),
            _ => (
                StatusCodes.Status500InternalServerError,
                "Internal Server Error",
                "An unexpected error occurred.",
                true
            )
        };

        if (shouldLogAsError)
        {
            _logger.LogError(exception, "Unhandled exception while processing request.");
        }
        else
        {
            _logger.LogWarning(exception, "Request failed with a handled exception.");
        }

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        };

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        await JsonSerializer.SerializeAsync(
            context.Response.Body,
            problemDetails,
            problemDetails.GetType(),
            JsonOptions
        );
    }

    private static async Task WriteValidationProblemAsync(
        HttpContext context,
        ApplicationValidationException exception
    )
    {
        var problemDetails = new ValidationProblemDetails(
            exception.Errors.ToDictionary(
                error => error.Key,
                error => error.Value,
                StringComparer.Ordinal
            )
        )
        {
            Type = "https://httpstatuses.com/400",
            Status = StatusCodes.Status400BadRequest,
            Title = "Validation failed",
            Detail = "One or more validation errors occurred.",
            Instance = context.Request.Path
        };

        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        context.Response.ContentType = "application/problem+json";

        await JsonSerializer.SerializeAsync(
            context.Response.Body,
            problemDetails,
            problemDetails.GetType(),
            JsonOptions
        );
    }
}
