using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sofra.Api.Helper;
using Sofra.Application.Common.Pagination;
using Sofra.Application.Common.Results;
using Sofra.Application.Constants;
using Sofra.Application.Features.Restaurants.Admin.GetApplication;
using Sofra.Application.Features.Restaurants.Admin.ListApplications;
using Sofra.Application.Features.Restaurants.Admin.UpdateApplicationStatus;
using Sofra.Application.Features.Restaurants.Dtos;
using Sofra.Domain.Enums;

namespace Sofra.Api.Controllers;

/// <summary>
/// Administrator review of restaurant applications (FR-47, FR-48): browse them, inspect one,
/// and decide it — approving materializes the restaurant, rejecting records a reason.
/// </summary>
[Route("api/v{version:apiVersion}/admin/restaurant-applications")]
[ApiController]
[Authorize(Roles = Roles.Administrator)]
public class RestaurantApplicationReviewController(IMediator mediator) : ControllerBase
{
    /// <summary>Lists restaurant applications with filters and paging.</summary>
    /// <param name="status">Only applications in this status; omit for all.</param>
    /// <param name="search">Case-insensitive match on the restaurant name.</param>
    /// <param name="pageNumber">Page number, starting at 1.</param>
    /// <param name="pageSize">Page size, 1-100.</param>
    /// <returns>A page of applications, newest first.</returns>
    /// <response code="200">The application page.</response>
    /// <response code="400">The filters failed validation.</response>
    /// <response code="401">The request is not authenticated.</response>
    /// <response code="403">The caller is not an administrator.</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ApplicationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ListApplications([FromQuery] RestaurantStatus? status, [FromQuery] string? search,
        [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new ListApplicationsQuery(new ApplicationListFilter(status, search, pageNumber, pageSize)), cancellationToken);
        if (result.IsFailure)
            return result.Error.ToActionResult(HttpContext);

        return Ok(result.Response);
    }

    /// <summary>Returns one restaurant application with the applicant's details.</summary>
    /// <param name="applicationId">The application id.</param>
    /// <returns>The application.</returns>
    /// <response code="200">The application.</response>
    /// <response code="401">The request is not authenticated.</response>
    /// <response code="403">The caller is not an administrator.</response>
    /// <response code="404">The application was not found.</response>
    [HttpGet("{applicationId:guid}")]
    [ProducesResponseType(typeof(ApplicationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetApplication(Guid applicationId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetApplicationQuery(applicationId), cancellationToken);
        if (result.IsFailure)
            return result.Error.ToActionResult(HttpContext);

        return Ok(result.Response);
    }

    /// <summary>Decides an application: approve it (materializes the restaurant and grants the Restaurant role) or reject it with a reason.</summary>
    /// <param name="applicationId">The application id.</param>
    /// <param name="command">The target status (Approved or Rejected) and the rejection reason when rejecting.</param>
    /// <returns>The updated application.</returns>
    /// <response code="200">The application was decided.</response>
    /// <response code="400">The request failed validation (field errors are returned).</response>
    /// <response code="401">The request is not authenticated.</response>
    /// <response code="403">The caller is not an administrator.</response>
    /// <response code="404">The application was not found.</response>
    /// <response code="422">The application was already decided.</response>
    [HttpPost("{applicationId:guid}/status")]
    [ProducesResponseType(typeof(ApplicationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdateApplicationStatus(Guid applicationId, UpdateApplicationStatusCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command with { ApplicationId = applicationId }, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToActionResult(HttpContext);

        return Ok(result.Response);
    }
}
