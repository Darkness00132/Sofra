using Mapster;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Sofra.Application.Common.Interfaces;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Authentication.Dtos;
using Sofra.Domain.Entities;

namespace Sofra.Application.Features.Authentication.RefreshAccessToken;

public class RefreshAccessTokenCommandHandler(
    UserManager<AppUser> userManager,
    IRepository<RefreshToken> refreshTokens,
    IUnitOfWork unitOfWork,
    ITokenProvider tokenProvider) : IRequestHandler<RefreshAccessTokenCommand, Result<LoginResponse>>
{
    public async Task<Result<LoginResponse>> Handle(RefreshAccessTokenCommand request, CancellationToken cancellationToken)
    {
        var storedToken = await refreshTokens.SingleOrDefaultAsync(
            t => t.Token == request.RefreshToken, cancellationToken);

        if (storedToken is null || !storedToken.IsActive)
            return Result<LoginResponse>.Failure(Error.Unauthorized(
                "INVALID_REFRESH_TOKEN",
                "The refresh token is invalid or expired."));

        var user = await userManager.FindByIdAsync(storedToken.UserId.ToString());
        if (user is null)
            return Result<LoginResponse>.Failure(Error.Unauthorized(
                "USER_NOT_FOUND",
                "The user for this session no longer exists."));

        storedToken.Revoke();

        var tokens = await tokenProvider.GenerateTokensAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new LoginResponse(
            tokens.AccessToken,
            tokens.RefreshToken,
            tokens.AccessTokenExpiresAtUtc,
            user.Adapt<AuthUserResponse>());
    }
}
