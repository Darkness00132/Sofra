using System.Security.Claims;
using Sofra.Application.Common.Interfaces;

namespace Sofra.Api.Services;

/// <summary>
/// HttpContext-backed implementation of <see cref="ICurrentUser"/>. A missing/unparseable user
/// id throws <see cref="UnauthorizedAccessException"/>, which the API maps to a 401 response.
/// </summary>
public class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public Guid GetUserId()
    {
        var value = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(value, out var userId))
            throw new UnauthorizedAccessException("You must be signed in to perform this operation.");

        return userId;
    }

    public bool IsInRole(string role) =>
        httpContextAccessor.HttpContext?.User?.IsInRole(role) ?? false;
}
