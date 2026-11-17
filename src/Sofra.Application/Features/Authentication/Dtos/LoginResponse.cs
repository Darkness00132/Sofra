namespace Sofra.Application.Features.Authentication.Dtos;

/// <summary>The signed-in user with their fresh tokens — shared by login, register and refresh.</summary>
/// <param name="AccessToken">The short-lived JWT sent on every request.</param>
/// <param name="RefreshToken">The long-lived token used to refresh the session.</param>
/// <param name="AccessTokenExpiresAtUtc">When the access token stops being valid (UTC).</param>
/// <param name="User">The signed-in user.</param>
public record LoginResponse(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAtUtc,
    AuthUserResponse User);
