using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Sofra.Api.Middleware;

namespace Sofra.Api.Helper;

/// <summary>
/// Static helper that generates the unified error response for every failed API call.
/// JSON shape: { status, title, code, detail, instance, traceId, errors? } — <c>errors</c> is
/// present only for validation failures. Both <see cref="ErrorExtensions.ToActionResult"/>
/// (controller errors) and the global exception handler build their responses through it.
/// </summary>
public static class ApiErrorResponseFactory
{
    public static ProblemDetails Create(
        HttpContext context,
        int status,
        string title,
        string code,
        string? detail,
        IDictionary<string, string[]>? errors = null)
    {
        var problem = new ProblemDetails
        {
            Status = status,
            Title = title,

            // never leak internals on server errors
            Detail = status >= StatusCodes.Status500InternalServerError
                ? "An unexpected error occurred."
                : detail,

            Instance = $"{context.Request.Method} {context.Request.Path}",
        };

        problem.Extensions["code"] = code;
        problem.Extensions["traceId"] = Activity.Current?.Id ?? context.TraceIdentifier;

        if (errors is { Count: > 0 })
            problem.Extensions["errors"] = errors;

        return problem;
    }
}
