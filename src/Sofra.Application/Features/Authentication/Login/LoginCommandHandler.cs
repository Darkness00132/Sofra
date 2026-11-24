using Mapster;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Sofra.Application.Common.Interfaces;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Authentication.Dtos;
using Sofra.Domain.Entities;

namespace Sofra.Application.Features.Authentication.Login;

public class LoginCommandHandler(
    UserManager<AppUser> userManager,
    ITokenProvider tokenProvider) : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null || !await userManager.CheckPasswordAsync(user, request.Password))
            return Result<LoginResponse>.Failure(Error.Unauthorized(
                "INVALID_CREDENTIALS",
                "Email or password is incorrect."));

        if (!user.EmailConfirmed)
            return Result<LoginResponse>.Failure(Error.Unauthorized(
                "EMAIL_NOT_CONFIRMED",
                "Please verify your email address before signing in."));

        if (await userManager.IsLockedOutAsync(user))
            return Result<LoginResponse>.Failure(Error.Unauthorized(
                "ACCOUNT_LOCKED",
                "This account has been locked. Contact support."));

        var tokens = await tokenProvider.GenerateTokensAsync(user, cancellationToken);

        return new LoginResponse(
            tokens.AccessToken,
            tokens.RefreshToken,
            tokens.AccessTokenExpiresAtUtc,
            user.Adapt<AuthUserResponse>());
    }
}
