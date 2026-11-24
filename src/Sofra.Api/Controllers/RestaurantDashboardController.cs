using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sofra.Api.Helper;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Orders.RestaurantDashboard;

namespace Sofra.Api.Controllers;

/// <summary>The signed-in owner's restaurant sales dashboard (FR-44).</summary>
[Route("api/v{version:apiVersion}/restaurants")]
[ApiController]
[Authorize]
public class RestaurantDashboardController(IMediator mediator) : ControllerBase
{
    /// <summary>The restaurant's sales dashboard: order counts and revenue (PRD: basic sales information).</summary>
    /// <returns>Order totals, pending/completed counts and revenue.</returns>
    /// <response code="200">The dashboard summary.</response>
    /// <response code="401">The request is not authenticated.</response>
    /// <response code="404">The user has no approved restaurant yet.</response>
    [HttpGet("dashboard")]
    [ProducesResponseType(typeof(OwnerDashboardResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRestaurantDashboard(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new RestaurantDashboardQuery(), cancellationToken);
        if (result.IsFailure)
            return result.Error.ToActionResult(HttpContext);

        return Ok(result.Response);
    }
}
