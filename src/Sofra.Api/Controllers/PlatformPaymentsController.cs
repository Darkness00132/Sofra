using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofra.Api.Helper;
using Sofra.Application.Common.Pagination;
using Sofra.Application.Constants;
using Sofra.Application.Features.Admin.ListPayments;
using Sofra.Domain.Enums;

namespace Sofra.Api.Controllers;

/// <summary>Administrator oversight of payment records.</summary>
[Route("api/v{version:apiVersion}/admin/payments")]
[ApiController]
[Authorize(Roles = Roles.Administrator)]
public class PlatformPaymentsController(IMediator mediator) : ControllerBase
{
    /// <summary>Lists payment records with filters and paging.</summary>
    /// <param name="status">Only payments in this status; omit for all.</param>
    /// <param name="method">Only payments with this method; omit for all.</param>
    /// <param name="pageNumber">Page number, starting at 1.</param>
    /// <param name="pageSize">Page size, 1-100.</param>
    /// <returns>A page of payments, newest paid first.</returns>
    /// <response code="200">The payment page.</response>
    /// <response code="400">The filters failed validation.</response>
    /// <response code="401">The request is not authenticated.</response>
    /// <response code="403">The caller is not an administrator.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<PaymentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ListPlatformPayments([FromQuery] PaymentStatus? status, [FromQuery] PaymentMethod? method,
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(
            new ListPaymentsQuery(new PaymentListFilter(status, method, pageNumber, pageSize)), cancellationToken);
        if (result.IsFailure)
            return result.Error.ToActionResult(HttpContext);

        return Ok(result.Response);
    }
}
