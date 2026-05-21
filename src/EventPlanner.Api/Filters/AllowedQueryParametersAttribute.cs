using EventPlanner.Application.Common.Validation;

using Microsoft.AspNetCore.Mvc.Filters;

namespace EventPlanner.Api.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class AllowedQueryParametersAttribute(params string[] allowedParameters) : Attribute, IActionFilter
{
    private readonly HashSet<string> _allowedParameters = new(allowedParameters, StringComparer.Ordinal);

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var errors = new ValidationErrorBag();

        foreach (var queryParameter in context.HttpContext.Request.Query.Keys)
        {
            if (_allowedParameters.Contains(queryParameter))
            {
                continue;
            }

            errors.Add(
                queryParameter,
                $"Query parameter '{queryParameter}' is not supported."
            );
        }

        errors.ThrowIfAny();
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}
