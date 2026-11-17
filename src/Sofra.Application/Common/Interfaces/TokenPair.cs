namespace Sofra.Application.Common.Interfaces;

/// <summary>The two tokens returned when a user signs in, registers, or refreshes a session.</summary>
/// <param name="AccessToken">The short-lived JWT sent on every request.</param>
/// <param name="RefreshToken">The long-lived token used to obtain a new pair.</param>
/// <param name="AccessTokenExpiresAtUtc">When the access token stops being valid (UTC).</param>
public record TokenPair(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAtUtc);
