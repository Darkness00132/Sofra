using MediatR;
using Sofra.Application.Common.Results;

namespace Sofra.Application.Features.Authentication.Logout;

/// <summary>Revokes a refresh token so it can no longer be used to obtain new token pairs.</summary>
public record LogoutCommand(string RefreshToken) : IRequest<Result>;
