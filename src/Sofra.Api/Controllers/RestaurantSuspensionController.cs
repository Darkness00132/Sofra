using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sofra.Api.Helper;
using Sofra.Application.Common.Pagination;
using Sofra.Application.Common.Results;
using Sofra.Application.Constants;
using Sofra.Application.Features.Restaurants.Admin.ListRestaurants;
using Sofra.Application.Features.Restaurants.Admin.UpdateRestaurantStatus;
using Sofra.Application.Features.Restaurants.Dtos;
using Sofra.Domain.Enums;

namespace Sofra.Api.Controllers;

/// <summary>
/// Administrator policing of restaurants (FR-49): browse them with filters, then suspend
/// or reinstate them — a suspended restaurant cannot accept orders (BR-09).
/// </summary>
[Route("api/v{version:apiVersion}/admin/restaurants")]
[ApiController]
[Authorize(Roles = Roles.Administrator)]
public class RestaurantSuspensionController(IMediator mediator) : ControllerBase
{
    /// <summary>Lists restaurants with filters and paging, for finding policing targets.</summary>
    /// <param name="status">Only restaurants in this status (Approved or Suspended); omit for all.</param>
    /// <param name="search">Case-insensitive match on the restaurant name.</param>
    /// <param name="pageNumber">Page number, starting at 1.</param>
    /// <param name="pageSize">Page size, 1-100.</param>
    /// <returns>A page of restaurants, ordered by name.</returns>
    /// <response code="200">The restaurant page.</response>
    /// <response code="400">The filters failed validation.</response>
    /// <response code="401">The request is not authenticated.</response>
    /// <response code="403">The caller is not an administrator.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<RestaurantResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ListRestaurants([FromQuery] RestaurantStatus? status, [FromQuery] string? search,
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new ListRestaurantsQuery(new RestaurantListFilter(status, search, pageNumber, pageSize)), cancellationToken);
        if (result.IsFailure)
            return result.Error.ToActionResult(HttpContext);

        return Ok(result.Response);
    }

    /// <summary>Suspends or reinstates a restaurant (a suspended restaurant cannot accept orders).</summary>
    /// <param name="restaurantId">The restaurant id.</param>
    /// <param name="command">The target status: Suspended or Approved (reinstated).</param>
    /// <returns>The updated restaurant.</returns>
    /// <response code="200">The restaurant status was updated.</response>
    /// <response code="400">The request failed validation (field errors are returned).</response>
    /// <response code="401">The request is not authenticated.</response>
    /// <response code="403">The caller is not an administrator.</response>
    /// <response code="404">The restaurant was not found.</response>
    /// <response code="422">The transition is not valid from the restaurant's current status.</response>
    [HttpPost("{restaurantId:guid}/status")]
    [ProducesResponseType(typeof(RestaurantResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdateRestaurantStatus(Guid restaurantId, UpdateRestaurantStatusCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command with { RestaurantId = restaurantId }, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToActionResult(HttpContext);

        return Ok(result.Response);
    }
}
