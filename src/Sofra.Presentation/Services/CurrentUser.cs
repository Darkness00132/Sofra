using System.Security.Claims;
using Sofra.Application.Common.Interfaces;
using Sofra.Application.Common.Results;

namespace Sofra.Api.Services;

/// <summary>
/// HttpContext-backed implementation of <see cref="ICurrentUser"/>. A missing/unparseable user
/// id yields an Unauthorized failure (never null), which handlers propagate as a 401 response.
/// </summary>
public class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public Result<Guid> GetUserId()
    {
        var value = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(value, out var userId))
            return Result.Failure<Guid>(Error.Unauthorized(
                "AUTHENTICATION_REQUIRED",
                "You must be signed in to perform this operation."));

        return userId;
    }

    public bool IsInRole(string role) =>
        httpContextAccessor.HttpContext?.User?.IsInRole(role) ?? false;
}
