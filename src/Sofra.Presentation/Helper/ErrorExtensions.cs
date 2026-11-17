using Microsoft.AspNetCore.Mvc;
using Sofra.Application.Common.Results;

namespace Sofra.Api.Helper
{
    public static class ErrorExtensions
    {
        /// <summary>
        /// Maps a failed <see cref="Result"/>'s <see cref="Error"/> to the unified error
        /// response. Controllers call this instead of inheriting a base controller.
        /// </summary>
        public static IActionResult ToActionResult(this Error error, HttpContext httpContext)
        {
            var (status, title) = error.Type switch
            {
                ErrorType.Validation => (StatusCodes.Status400BadRequest, "Validation Error"),
                ErrorType.Unauthorized => (StatusCodes.Status401Unauthorized, "Authentication Required"),
                ErrorType.Forbidden => (StatusCodes.Status403Forbidden, "Access Denied"),
                ErrorType.NotFound => (StatusCodes.Status404NotFound, "Not Found"),
                ErrorType.Conflict => (StatusCodes.Status409Conflict, "Conflict"),
                ErrorType.BusinessRule => (StatusCodes.Status422UnprocessableEntity, "Business Rule Violated"),
                ErrorType.InternalFailure => (StatusCodes.Status500InternalServerError, "Internal Server Error"),
                _ => (StatusCodes.Status500InternalServerError, "Internal Server Error")
            };

            var problemDetails = ApiErrorResponseFactory.Create(
                httpContext, status, title, error.Code, error.Description);

            return new ObjectResult(problemDetails)
            {
                StatusCode = status
            };
        }
    }
}
