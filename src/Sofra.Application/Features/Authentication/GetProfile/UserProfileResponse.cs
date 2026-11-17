namespace Sofra.Application.Features.Authentication.GetProfile;

/// <summary>The signed-in user's profile information.</summary>
public record UserProfileResponse(
    Guid Id,
    string Email,
    string? PhoneNumber,
    IReadOnlyList<string> Roles);
