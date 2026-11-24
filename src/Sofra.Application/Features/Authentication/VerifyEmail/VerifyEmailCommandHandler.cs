using MediatR;
using Microsoft.AspNetCore.Identity;
using Sofra.Application.Common.Results;
using Sofra.Domain.Entities;

namespace Sofra.Application.Features.Authentication.VerifyEmail;

public class VerifyEmailCommandHandler(UserManager<AppUser> userManager) : IRequestHandler<VerifyEmailCommand, Result>
{
    public async Task<Result> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null)
            return Result.Failure(Error.NotFound("USER_NOT_FOUND", "No account matches this email."));

        if (user.EmailConfirmed)
            return Result.Success();

        var result = await userManager.ConfirmEmailAsync(user, request.Token);
        if (!result.Succeeded)
        {
            var description = string.Join(" ", result.Errors.Select(e => e.Description));
            return Result.Failure(Error.Validation("EMAIL_VERIFICATION_FAILED", description));
        }

        return Result.Success();
    }
}
