using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofra.Api.Helper;
using Sofra.Application.Constants;
using Sofra.Application.Features.Admin.PlatformDashboard;

namespace Sofra.Api.Controllers;

/// <summary>Platform-wide statistics for the administrator dashboard.</summary>
[Route("api/v{version:apiVersion}/admin/dashboard")]
[ApiController]
[Authorize(Roles = Roles.Administrator)]
public class PlatformDashboardController(IMediator mediator) : ControllerBase
{
    /// <summary>Returns the platform overview: restaurant, application, account and order statistics.</summary>
    /// <returns>The platform statistics.</returns>
    /// <response code="200">The dashboard summary.</response>
    /// <response code="401">The request is not authenticated.</response>
    /// <response code="403">The caller is not an administrator.</response>
    [HttpGet]
    [ProducesResponseType(typeof(AdminDashboardResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetPlatformDashboard(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new PlatformDashboardQuery(), cancellationToken);
        if (result.IsFailure)
            return result.Error.ToActionResult(HttpContext);

        return Ok(result.Response);
    }
}
