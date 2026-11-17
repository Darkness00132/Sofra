using Mapster;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Sofra.Application.Common.Interfaces;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Authentication.Dtos;
using Sofra.Application.Constants;
using Sofra.Domain.Entities;

namespace Sofra.Application.Features.Authentication.Register;

public class RegisterCommandHandler(
    UserManager<AppUser> userManager,
    ITokenProvider tokenProvider) : IRequestHandler<RegisterCommand, Result<LoginResponse>>
{
    public async Task<Result<LoginResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (await userManager.FindByEmailAsync(request.Email) is not null)
            return Result<LoginResponse>.Failure(Error.Conflict(
                "EMAIL_ALREADY_REGISTERED",
                "An account with this email already exists."));

        var user = new AppUser { UserName = request.Email, Email = request.Email };

        var createResult = await userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
        {
            var description = string.Join(" ", createResult.Errors.Select(e => e.Description));
            return Result<LoginResponse>.Failure(Error.Validation(
                "REGISTRATION_INVALID",
                description));
        }

        await userManager.AddToRoleAsync(user, Roles.Customer);

        var tokens = await tokenProvider.GenerateTokensAsync(user, cancellationToken);

        return new LoginResponse(
            tokens.AccessToken,
            tokens.RefreshToken,
            tokens.AccessTokenExpiresAtUtc,
            user.Adapt<AuthUserResponse>());
    }
}
