using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofra.Api.Helper;
using Sofra.Application.Features.Authentication.GetProfile;
using Sofra.Application.Features.Authentication.UpdateProfile;

namespace Sofra.Api.Controllers;

/// <summary>The signed-in user's profile: read and update (FR-03).</summary>
[Route("api/v{version:apiVersion}/auth")]
[ApiController]
public class ProfileController(IMediator mediator) : ControllerBase
{
    /// <summary>Returns the profile information of the currently signed-in user.</summary>
    /// <returns>The signed-in user's id, email, phone number and roles.</returns>
    /// <response code="200">The user's profile information.</response>
    /// <response code="401">The request is not authenticated.</response>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyProfile(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetProfileQuery(), cancellationToken);
        if (result.IsFailure)
            return result.Error.ToActionResult(HttpContext);

        return Ok(result.Response);
    }

    /// <summary>Updates the signed-in user's profile information (FR-03).</summary>
    /// <param name="command">The updated profile fields.</param>
    /// <returns>The updated profile.</returns>
    /// <response code="200">The profile was updated.</response>
    /// <response code="400">The request failed validation (field errors are returned).</response>
    /// <response code="401">The request is not authenticated.</response>
    /// <response code="404">The user no longer exists.</response>
    [HttpPut("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateMyProfile(UpdateProfileCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToActionResult(HttpContext);

        return Ok(result.Response);
    }
}
