using EventPlanner.Application.Common.Exceptions;
using EventPlanner.Domain.Common;

using Microsoft.AspNetCore.Mvc;

namespace EventPlanner.Api.Middleware;

public sealed class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger
    )
{
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
            ApplicationValidationException => (
                StatusCodes.Status400BadRequest,
                "Bad Request",
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

        await context.Response.WriteAsJsonAsync(problemDetails);
    }
}
