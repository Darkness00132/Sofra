using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofra.Api.Helper;
using Sofra.Application.Common.Pagination;
using Sofra.Application.Constants;
using Sofra.Application.Features.Admin.GetOrderDetails;
using Sofra.Application.Features.Admin.ListOrders;
using Sofra.Application.Features.Orders;
using Sofra.Application.Features.Orders.Dtos;
using Sofra.Domain.Enums;

namespace Sofra.Api.Controllers;

/// <summary>Administrator oversight of all platform orders (FR-45, FR-51).</summary>
[Route("api/v{version:apiVersion}/admin/orders")]
[ApiController]
[Authorize(Roles = Roles.Administrator)]
public class PlatformOrdersController(IMediator mediator) : ControllerBase
{
    /// <summary>Lists all platform orders with filters and paging (FR-45, FR-51).</summary>
    /// <param name="restaurantId">Only orders of this restaurant.</param>
    /// <param name="customerId">Only orders of this customer.</param>
    /// <param name="status">Only orders in this status; omit for all.</param>
    /// <param name="from">Only orders created on or after this date (UTC).</param>
    /// <param name="to">Only orders created on or before this date (UTC).</param>
    /// <param name="pageNumber">Page number, starting at 1.</param>
    /// <param name="pageSize">Page size, 1-100.</param>
    /// <returns>A page of orders, newest first.</returns>
    /// <response code="200">The order page.</response>
    /// <response code="400">The filters failed validation.</response>
    /// <response code="401">The request is not authenticated.</response>
    /// <response code="403">The caller is not an administrator.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<OrderSummaryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ListPlatformOrders([FromQuery] Guid? restaurantId, [FromQuery] Guid? customerId,
        [FromQuery] OrderStatus? status, [FromQuery] DateTime? from, [FromQuery] DateTime? to,
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(
            new ListOrdersQuery(new AdminOrderListFilter(restaurantId, customerId, status, from, to, pageNumber, pageSize)),
            cancellationToken);
        if (result.IsFailure)
            return result.Error.ToActionResult(HttpContext);

        return Ok(result.Response);
    }

    /// <summary>Returns full details of any order: items, totals, payment and status history (FR-45, FR-51).</summary>
    /// <param name="orderId">The order id.</param>
    /// <returns>The order details.</returns>
    /// <response code="200">The order details.</response>
    /// <response code="401">The request is not authenticated.</response>
    /// <response code="403">The caller is not an administrator.</response>
    /// <response code="404">The order was not found.</response>
    [HttpGet("{orderId:guid}")]
    [ProducesResponseType(typeof(OrderDetailsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrder(Guid orderId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetOrderDetailsQuery(orderId), cancellationToken);
        if (result.IsFailure)
            return result.Error.ToActionResult(HttpContext);

        return Ok(result.Response);
    }
}
