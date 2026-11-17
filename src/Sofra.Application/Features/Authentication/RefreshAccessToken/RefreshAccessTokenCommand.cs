using MediatR;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Authentication.Dtos;

namespace Sofra.Application.Features.Authentication.RefreshAccessToken;

/// <summary>Exchanges a still-active refresh token for a new token pair, revoking the old refresh token.</summary>
public record RefreshAccessTokenCommand(string RefreshToken) : IRequest<Result<LoginResponse>>;
