using System.Text.Json;

using Microsoft.AspNetCore.Mvc;

namespace EventPlanner.Api.Errors;

public static class ProblemDetailsResponseWriter
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public static Task WriteAsync(HttpContext context, ProblemDetails problemDetails)
    {
        context.Response.StatusCode =
            problemDetails.Status ?? StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/problem+json";

        return JsonSerializer.SerializeAsync(
            context.Response.Body,
            problemDetails,
            problemDetails.GetType(),
            JsonOptions
        );
    }
}
