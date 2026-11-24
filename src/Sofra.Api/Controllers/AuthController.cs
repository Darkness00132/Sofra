using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofra.Api.Helper;
using Sofra.Application.Features.Authentication.Dtos;
using Sofra.Application.Features.Authentication.ForgotPassword;
using Sofra.Application.Features.Authentication.Login;
using Sofra.Application.Features.Authentication.Logout;
using Sofra.Application.Features.Authentication.RefreshAccessToken;
using Sofra.Application.Features.Authentication.Register;
using Sofra.Application.Features.Authentication.ResetPassword;
using Sofra.Application.Features.Authentication.VerifyEmail;

namespace Sofra.Api.Controllers;

/// <summary>Authentication endpoints: register, login, token refresh, logout, password reset and profile.</summary>
[Route("api/v{version:apiVersion}/auth")]
[ApiController]
public class AuthController(IMediator mediator) : ControllerBase
{
    /// <summary>Registers a new customer account and immediately signs the user in.</summary>
    /// <param name="command">The registration details (email and password).</param>
    /// <returns>An access token, a refresh token and the created user.</returns>
    /// <response code="200">Account created; the token pair is returned.</response>
    /// <response code="400">The request failed validation (field errors are returned).</response>
    /// <response code="409">An account with this email already exists.</response>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(RegisterCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToActionResult(HttpContext);

        return Ok(result.Response);
    }

    /// <summary>Signs a user in with email and password.</summary>
    /// <param name="command">The login credentials.</param>
    /// <returns>An access token, a refresh token and the signed-in user.</returns>
    /// <response code="200">Signed in; the token pair is returned.</response>
    /// <response code="400">The request failed validation (field errors are returned).</response>
    /// <response code="401">The email or password is incorrect.</response>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(LoginCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToActionResult(HttpContext);

        return Ok(result.Response);
    }

    /// <summary>Exchanges a still-active refresh token for a new token pair (the old refresh token is revoked).</summary>
    /// <param name="command">The refresh token to exchange.</param>
    /// <returns>A new access token with a new refresh token.</returns>
    /// <response code="200">A new token pair is returned.</response>
    /// <response code="400">The request failed validation (field errors are returned).</response>
    /// <response code="401">The refresh token is invalid or expired.</response>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh(RefreshAccessTokenCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToActionResult(HttpContext);

        return Ok(result.Response);
    }

    /// <summary>Revokes a refresh token so it can no longer be used to obtain new token pairs.</summary>
    /// <param name="command">The refresh token to revoke.</param>
    /// <response code="204">The refresh token was revoked (or was already inactive).</response>
    /// <response code="400">The request failed validation (field errors are returned).</response>
    /// <response code="404">The refresh token does not exist.</response>
    [HttpPost("logout")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Logout(LogoutCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToActionResult(HttpContext);

        return NoContent();
    }

    /// <summary>Emails a one-time 6-digit password reset code to the user (valid for 10 minutes).</summary>
    /// <param name="command">The registered email address.</param>
    /// <remarks>Always returns 200, whether or not the email is registered, so the endpoint never reveals account existence.</remarks>
    /// <response code="200">The request was processed; the code is emailed when the account exists.</response>
    /// <response code="400">The request failed validation (field errors are returned).</response>
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToActionResult(HttpContext);

        return Ok();
    }

    /// <summary>Resets the password using the one-time code emailed by the forgot-password flow.</summary>
    /// <param name="command">The email, the 6-digit reset code and the new password.</param>
    /// <response code="204">The password was reset; the reset code is consumed.</response>
    /// <response code="400">The request failed validation (field errors are returned).</response>
    /// <response code="422">The reset code is invalid or has expired.</response>
    [HttpPost("reset-password")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> ResetPassword(ResetPasswordCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToActionResult(HttpContext);

        return NoContent();
    }

    /// <summary>Confirms an email address using the token issued at registration.</summary>
    /// <param name="command">The email and the confirmation token.</param>
    /// <response code="204">The email was verified.</response>
    /// <response code="400">The request failed validation.</response>
    /// <response code="404">No account matches the email.</response>
    /// <response code="422">The token is invalid or expired.</response>
    [HttpPost("verify-email")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> VerifyEmail(VerifyEmailCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        if (result.IsFailure)
            return result.Error.ToActionResult(HttpContext);

        return NoContent();
    }

}
