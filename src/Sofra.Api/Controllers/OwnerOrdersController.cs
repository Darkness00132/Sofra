using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sofra.Api.Helper;
using Sofra.Application.Common.Pagination;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Orders.Dtos;
using Sofra.Application.Features.Orders.GetRestaurantOrder;
using Sofra.Application.Features.Orders.ListRestaurantOrders;
using Sofra.Application.Features.Orders.UpdateOrderStatus;
using Sofra.Domain.Enums;
using Sofra.Application.Features.Orders;

namespace Sofra.Api.Controllers;

/// <summary>The signed-in owner's incoming orders and their processing (FR-32…FR-38, FR-44).</summary>
[Route("api/v{version:apiVersion}/restaurants")]
[ApiController]
[Authorize]
public class OwnerOrdersController(IMediator mediator) : ControllerBase
{
    /// <summary>Lists the owner's restaurant orders with a status filter and paging (FR-32, FR-44).</summary>
    /// <param name="status">Only orders in this status (e.g. Pending for incoming, Completed for history); omit for all.</param>
    /// <param name="pageNumber">Page number, starting at 1.</param>
    /// <param name="pageSize">Page size, 1-100.</param>
    /// <returns>A page of orders, newest first.</returns>
    /// <response code="200">The order page.</response>
    /// <response code="400">The filters failed validation.</response>
    /// <response code="401">The request is not authenticated.</response>
    /// <response code="404">The user has no approved restaurant yet.</response>
    [HttpGet("orders")]
    [ProducesResponseType(typeof(PagedResult<OrderSummaryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ListOwnerOrders([FromQuery] OrderStatus? status,
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(
            new ListRestaurantOrdersQuery(new RestaurantOrdersFilter(status, pageNumber, pageSize)), cancellationToken);
        if (result.IsFailure)
            return result.Error.ToActionResult(HttpContext);

        return Ok(result.Response);
    }

    /// <summary>Returns full details of one of the restaurant's orders: items, totals and status history (BR-03, BR-10).</summary>
    /// <param name="orderId">The order id.</param>
    /// <returns>The order details.</returns>
    /// <response code="200">The order details.</response>
    /// <response code="401">The request is not authenticated.</response>
    /// <response code="404">The user has no approved restaurant, or the order belongs to another restaurant.</response>
    [HttpGet("orders/{orderId:guid}")]
    [ProducesResponseType(typeof(OrderDetailsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrder(Guid orderId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetRestaurantOrderQuery(orderId), cancellationToken);
        if (result.IsFailure)
            return result.Error.ToActionResult(HttpContext);

        return Ok(result.Response);
    }

    /// <summary>Moves one of the restaurant's orders to its next status: Accepted, Rejected (reason required), Preparing, Ready or Completed (FR-33…FR-35, BR-06, BR-08). Cash payments complete with the order.</summary>
    /// <param name="orderId">The order id.</param>
    /// <param name="command">The target status and the rejection reason when rejecting.</param>
    /// <returns>The updated order details.</returns>
    /// <response code="200">The order status was updated.</response>
    /// <response code="400">The request failed validation (field errors are returned).</response>
    /// <response code="401">The request is not authenticated.</response>
    /// <response code="404">The user has no approved restaurant, or the order belongs to another restaurant.</response>
    /// <response code="422">The transition is not valid from the order's current status (BR-06, BR-08).</response>
    [HttpPost("orders/{orderId:guid}/status")]
    [ProducesResponseType(typeof(OrderDetailsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdateOrderStatus(Guid orderId, UpdateOrderStatusCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command with { OrderId = orderId }, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToActionResult(HttpContext);

        return Ok(result.Response);
    }
}
