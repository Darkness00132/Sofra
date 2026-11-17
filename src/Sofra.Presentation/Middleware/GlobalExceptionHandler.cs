using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Sofra.Api.Helper;
using Sofra.Domain.Exceptions;

namespace Sofra.Api.Middleware;

/// <summary>
/// Global exception handler for ANY exception that escapes the pipeline. Registered in
/// Program.cs with <c>AddExceptionHandler&lt;GlobalExceptionHandler&gt;()</c> and enabled with
/// <c>app.UseExceptionHandler()</c>. Every response goes through
/// <see cref="ApiErrorResponseFactory.Create"/> so the JSON shape is always the same.
/// </summary>
public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext context,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (context.Response.HasStarted)
            return false;

        int status;
        string title, code, detail;
        IDictionary<string, string[]>? errors = null;

        if (exception is ValidationException validation)
        {
            status = StatusCodes.Status400BadRequest;
            title = "Validation Error";
            code = "General.Validation";
            detail = "One or more validation errors occurred.";
            errors = validation.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
        }
        else if (exception is DomainException domain)
        {
            status = StatusCodes.Status422UnprocessableEntity;
            title = "Business Rule Violated";
            code = "BUSINESS_RULE_VIOLATION";
            detail = domain.Message;
        }
        else
        {
            logger.LogError(exception, "Unhandled exception processing {Method} {Path}",
                context.Request.Method, context.Request.Path);

            status = StatusCodes.Status500InternalServerError;
            title = "Internal Server Error";
            code = "INTERNAL_SERVER_ERROR";
            detail = "An unexpected error occurred.";
        }

        var problem = ApiErrorResponseFactory.Create(context, status, title, code, detail, errors);

        context.Response.StatusCode = status;
        await context.Response.WriteAsJsonAsync(problem, cancellationToken);
        return true;
    }
}
