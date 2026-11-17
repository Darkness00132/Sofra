namespace Sofra.Application.Features.Authentication.Dtos;

/// <summary>The public identity of the signed-in user.</summary>
public record AuthUserResponse(
    Guid Id,
    string Email);
