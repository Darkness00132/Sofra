using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sofra.Api.Helper;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Restaurants.Apply;
using Sofra.Application.Features.Restaurants.Dtos;
using Sofra.Application.Features.Restaurants.MyApplications;

namespace Sofra.Api.Controllers;

/// <summary>Submitting and tracking the signed-in user's own restaurant applications.</summary>
[Route("api/v{version:apiVersion}/restaurants")]
[ApiController]
public class MyRestaurantApplicationsController(IMediator mediator) : ControllerBase
{
    /// <summary>Submits a restaurant application for administrator review.</summary>
    /// <param name="command">The restaurant's name, address, phone and description.</param>
    /// <returns>The created application with its PendingApproval status.</returns>
    /// <response code="200">Application submitted.</response>
    /// <response code="400">The request failed validation (field errors are returned).</response>
    /// <response code="401">The request is not authenticated.</response>
    /// <response code="409">An application from this user is already pending review.</response>
    [HttpPost("apply")]
    [Authorize]
    [ProducesResponseType(typeof(ApplicationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ApplyForRestaurant(ApplyApplicationCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToActionResult(HttpContext);

        return Ok(result.Response);
    }

    /// <summary>Lists the signed-in user's own restaurant applications, newest first.</summary>
    /// <returns>The user's applications with their review status.</returns>
    /// <response code="200">The user's applications.</response>
    /// <response code="401">The request is not authenticated.</response>
    [HttpGet("my-applications")]
    [Authorize]
    [ProducesResponseType(typeof(IReadOnlyList<ApplicationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ListMyApplications(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new ListMyApplicationsQuery(), cancellationToken);
        if (result.IsFailure)
            return result.Error.ToActionResult(HttpContext);

        return Ok(result.Response);
    }
}
