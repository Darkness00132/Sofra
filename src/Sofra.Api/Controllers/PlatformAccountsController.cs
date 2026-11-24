using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofra.Api.Helper;
using Sofra.Application.Common.Pagination;
using Sofra.Application.Constants;
using Sofra.Application.Features.Admin.ListAccounts;
using Sofra.Application.Features.Admin.SetAccountLock;

namespace Sofra.Api.Controllers;

/// <summary>Administrator view of user accounts (FR-50).</summary>
[Route("api/v{version:apiVersion}/admin/accounts")]
[ApiController]
[Authorize(Roles = Roles.Administrator)]
public class PlatformAccountsController(IMediator mediator) : ControllerBase
{
    /// <summary>Lists user accounts with role filter, search and paging.</summary>
    /// <param name="role">Only accounts holding this role (Customer, Restaurant or Administrator); omit for all.</param>
    /// <param name="search">Case-insensitive match on the account email.</param>
    /// <param name="pageNumber">Page number, starting at 1.</param>
    /// <param name="pageSize">Page size, 1-100.</param>
    /// <returns>A page of accounts, ordered by email.</returns>
    /// <response code="200">The account page.</response>
    /// <response code="400">The filters failed validation.</response>
    /// <response code="401">The request is not authenticated.</response>
    /// <response code="403">The caller is not an administrator.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<AdminAccountResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ListPlatformAccounts([FromQuery] string? role, [FromQuery] string? search,
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(
            new ListAccountsQuery(new AdminAccountFilter(role, search, pageNumber, pageSize)), cancellationToken);
        if (result.IsFailure)
            return result.Error.ToActionResult(HttpContext);

        return Ok(result.Response);
    }

    /// <summary>Locks or unlocks a user account. Locked accounts cannot sign in (FR-50).</summary>
    /// <param name="accountId">The account id.</param>
    /// <param name="command">The lock flag.</param>
    /// <response code="200">The account lock state was updated.</response>
    /// <response code="401">The request is not authenticated.</response>
    /// <response code="403">The caller is not an administrator.</response>
    /// <response code="404">The account was not found.</response>
    /// <response code="422">A business rule blocked the change (e.g. locking your own account).</response>
    [HttpPatch("{accountId:guid}/lock")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> SetAccountLock(Guid accountId, SetAccountLockCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command with { AccountId = accountId }, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToActionResult(HttpContext);

        return Ok();
    }
}
