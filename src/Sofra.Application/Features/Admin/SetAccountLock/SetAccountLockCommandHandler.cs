using MediatR;
using Microsoft.AspNetCore.Identity;
using Sofra.Application.Common.Interfaces;
using Sofra.Application.Common.Results;
using Sofra.Domain.Entities;

namespace Sofra.Application.Features.Admin.SetAccountLock;

public class SetAccountLockCommandHandler(
    UserManager<AppUser> userManager,
    ICurrentUser currentUser) : IRequestHandler<SetAccountLockCommand, Result>
{
    public async Task<Result> Handle(SetAccountLockCommand request, CancellationToken cancellationToken)
    {
        var adminId = currentUser.GetUserId();

        var user = await userManager.FindByIdAsync(request.AccountId.ToString());
        if (user is null)
            return Result.Failure(Error.NotFound(
                "ACCOUNT_NOT_FOUND",
                "The account was not found."));

        if (user.Id == adminId)
            return Result.Failure(Error.BusinessRule(
                "CANNOT_LOCK_SELF",
                "You cannot lock your own account."));

        IdentityResult lockResult;

        if (request.Locked)
        {
            lockResult = await userManager.SetLockoutEnabledAsync(user, true);
            if (lockResult.Succeeded)
                lockResult = await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
        }
        else
        {
            lockResult = await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);
        }

        if (!lockResult.Succeeded)
        {
            var description = string.Join(" ", lockResult.Errors.Select(e => e.Description));
            return Result.Failure(Error.Internal("ACCOUNT_LOCK_FAILED", description));
        }

        return Result.Success();
    }
}
