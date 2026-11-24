using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sofra.Api.Helper;
using Sofra.Application.Common.Pagination;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Orders.CancelOrder;
using Sofra.Application.Features.Orders.Dtos;
using Sofra.Application.Features.Orders.GetOrderDetails;
using Sofra.Application.Features.Orders.ListMyOrders;
using Sofra.Application.Features.Orders.PlaceOrder;
using Sofra.Application.Features.Orders.PreviewOrder;
using Sofra.Domain.Enums;
using Sofra.Application.Features.Orders;

namespace Sofra.Api.Controllers;

/// <summary>Checkout, tracking and cancellation of the signed-in customer's orders (FR-25…FR-43).</summary>
[Route("api/v{version:apiVersion}/orders")]
[ApiController]
[Authorize]
public class CustomerOrdersController(IMediator mediator) : ControllerBase
{
    /// <summary>Checks out the cart as an order: re-validates items, snapshots prices, applies fees and records the payment (FR-25…FR-31).</summary>
    /// <param name="command">The fulfilment method, payment method and delivery address for delivery orders.</param>
    /// <returns>The placed order with its totals.</returns>
    /// <response code="200">The order was placed and the cart cleared.</response>
    /// <response code="400">The request failed validation (field errors are returned).</response>
    /// <response code="401">The request is not authenticated.</response>
    /// <response code="422">A business rule blocked the checkout (empty cart, restaurant not accepting, item unavailable).</response>
    [HttpPost]
    [ProducesResponseType(typeof(PlaceOrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> PlaceOrder(PlaceOrderCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToActionResult(HttpContext);

        return Ok(result.Response);
    }

    /// <summary>Lists the signed-in customer's own orders with filters and paging (FR-36, FR-43).</summary>
    /// <param name="status">Only orders in this status; omit for all.</param>
    /// <param name="from">Only orders created on or after this date (UTC).</param>
    /// <param name="to">Only orders created on or before this date (UTC).</param>
    /// <param name="pageNumber">Page number, starting at 1.</param>
    /// <param name="pageSize">Page size, 1-100.</param>
    /// <returns>A page of orders, newest first.</returns>
    /// <response code="200">The order page.</response>
    /// <response code="400">The filters failed validation.</response>
    /// <response code="401">The request is not authenticated.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<OrderSummaryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ListMyOrders([FromQuery] OrderStatus? status, [FromQuery] DateTime? from, [FromQuery] DateTime? to,
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(
            new ListMyOrdersQuery(new MyOrdersFilter(status, from, to, pageNumber, pageSize)), cancellationToken);
        if (result.IsFailure)
            return result.Error.ToActionResult(HttpContext);

        return Ok(result.Response);
    }

    /// <summary>Returns full details of one of the customer's own orders: items, totals, payment and status history (FR-36, FR-43, BR-10).</summary>
    /// <param name="orderId">The order id.</param>
    /// <returns>The order details.</returns>
    /// <response code="200">The order details.</response>
    /// <response code="401">The request is not authenticated.</response>
    /// <response code="404">The order was not found among this customer's orders.</response>
    [HttpGet("{orderId:guid}")]
    [ProducesResponseType(typeof(OrderDetailsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrder(Guid orderId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetOrderDetailsQuery(orderId), cancellationToken);
        if (result.IsFailure)
            return result.Error.ToActionResult(HttpContext);

        return Ok(result.Response);
    }

    /// <summary>Cancels the customer's order while the cancellation policy allows it (FR-39…FR-41, BR-07).</summary>
    /// <param name="orderId">The order id.</param>
    /// <param name="command">The cancellation reason and optional note.</param>
    /// <returns>The cancelled order details.</returns>
    /// <response code="200">The order was cancelled.</response>
    /// <response code="400">The request failed validation (field errors are returned).</response>
    /// <response code="401">The request is not authenticated.</response>
    /// <response code="404">The order was not found among this customer's orders.</response>
    /// <response code="422">The cancellation policy does not allow cancelling this order.</response>
    [HttpPost("{orderId:guid}/cancel")]
    [ProducesResponseType(typeof(OrderDetailsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CancelOrder(Guid orderId, CancelOrderCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command with { OrderId = orderId }, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToActionResult(HttpContext);

        return Ok(result.Response);
    }

    /// <summary>Computes the exact charges the checkout would produce for the current cart — without placing the order (FR-25).</summary>
    /// <param name="command">The fulfilment method and delivery address being considered.</param>
    /// <returns>The subtotal, tax, delivery fee and total the checkout would charge.</returns>
    /// <response code="200">The checkout preview.</response>
    /// <response code="400">The request failed validation (field errors are returned).</response>
    /// <response code="401">The request is not authenticated.</response>
    /// <response code="422">A business rule blocked the preview (empty cart, restaurant not accepting, item unavailable).</response>
    [HttpPost("preview")]
    [Authorize]
    [ProducesResponseType(typeof(OrderPreviewResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> PreviewOrder(PreviewOrderCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToActionResult(HttpContext);

        return Ok(result.Response);
    }
}
