using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sofra.Api.Helper;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Menus.Dtos;
using Sofra.Application.Features.Menus.GetRestaurantMenu;

namespace Sofra.Api.Controllers;

/// <summary>Public menu of an approved restaurant — available items only, grouped by menu type (FR-07).</summary>
[Route("api/v{version:apiVersion}/restaurants")]
[ApiController]
public class RestaurantMenuController(IMediator mediator) : ControllerBase
{
    /// <summary>Returns a restaurant's public menu — available items only, grouped by menu type (FR-07).</summary>
    /// <param name="restaurantId">The restaurant id.</param>
    /// <returns>The menu grouped by menu type.</returns>
    /// <response code="200">The restaurant's menu.</response>
    /// <response code="404">The restaurant was not found (or is not approved).</response>
    [HttpGet("{restaurantId:guid}/menu")]
    [ProducesResponseType(typeof(RestaurantMenuResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRestaurantMenu(Guid restaurantId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetRestaurantMenuQuery(restaurantId), cancellationToken);
        if (result.IsFailure)
            return result.Error.ToActionResult(HttpContext);

        return Ok(result.Response);
    }
}
