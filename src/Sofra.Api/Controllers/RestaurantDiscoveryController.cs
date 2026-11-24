using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sofra.Api.Helper;
using Sofra.Application.Common.Pagination;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Discovery.Dtos;
using Sofra.Application.Features.Discovery.GetRestaurantDetails;
using Sofra.Application.Features.Discovery.ListRestaurants;
using Sofra.Application.Features.Discovery;

namespace Sofra.Api.Controllers;

/// <summary>Public browsing of approved restaurants and their menus (FR-06…FR-10). No authentication required.</summary>
[Route("api/v{version:apiVersion}/restaurants")]
[ApiController]
public class RestaurantDiscoveryController(IMediator mediator) : ControllerBase
{
    /// <summary>Lists approved restaurants with search, category filter and paging.</summary>
    /// <param name="search">Case-insensitive match on the restaurant name (FR-08).</param>
    /// <param name="categoryId">Only restaurants with a menu type under this platform category (FR-09).</param>
    /// <param name="acceptingOrders">Only restaurants currently accepting orders.</param>
    /// <param name="pageNumber">Page number, starting at 1.</param>
    /// <param name="pageSize">Page size, 1-100.</param>
    /// <returns>A page of approved restaurants, ordered by name.</returns>
    /// <response code="200">The restaurant page.</response>
    /// <response code="400">The filters failed validation.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<PublicRestaurantResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListRestaurants([FromQuery] string? search, [FromQuery] Guid? categoryId,
        [FromQuery] bool? acceptingOrders, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(
            new ListRestaurantsQuery(new PublicRestaurantFilter(search, categoryId, acceptingOrders, pageNumber, pageSize)),
            cancellationToken);
        if (result.IsFailure)
            return result.Error.ToActionResult(HttpContext);

        return Ok(result.Response);
    }

    /// <summary>Returns one approved restaurant's details.</summary>
    /// <param name="restaurantId">The restaurant id.</param>
    /// <returns>The restaurant's details and order-acceptance indicator (FR-10).</returns>
    /// <response code="200">The restaurant details.</response>
    /// <response code="404">The restaurant was not found (or is not approved).</response>
    [HttpGet("{restaurantId:guid}")]
    [ProducesResponseType(typeof(PublicRestaurantResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRestaurant(Guid restaurantId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetRestaurantDetailsQuery(restaurantId), cancellationToken);
        if (result.IsFailure)
            return result.Error.ToActionResult(HttpContext);

        return Ok(result.Response);
    }

}
