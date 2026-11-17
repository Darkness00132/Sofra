using MediatR;
using Microsoft.AspNetCore.Identity;
using Sofra.Application.Common.Interfaces;
using Sofra.Application.Common.Results;
using Sofra.Domain.Entities;

namespace Sofra.Application.Features.Authentication.ResetPassword;

public class ResetPasswordCommandHandler(
    UserManager<AppUser> userManager,
    IRepository<PasswordResetOtp> resetCodes,
    IUnitOfWork unitOfWork) : IRequestHandler<ResetPasswordCommand, Result>
{
    public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null)
            return InvalidCode();

        var codeHash = PasswordResetOtp.Hash(request.Otp);
        var resetCode = await resetCodes.SingleOrDefaultAsync(
            o => o.UserId == user.Id
                 && o.CodeHash == codeHash
                 && o.UsedAt == null
                 && o.ExpiresAt > DateTime.UtcNow,
            cancellationToken);

        if (resetCode is null)
            return InvalidCode();

        resetCode.MarkUsed();

        var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
        var resetResult = await userManager.ResetPasswordAsync(user, resetToken, request.NewPassword);
        if (!resetResult.Succeeded)
        {
            var description = string.Join(" ", resetResult.Errors.Select(e => e.Description));
            return Result.Failure(Error.Validation("PASSWORD_INVALID", description));
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private static Result InvalidCode() => Result.Failure(Error.BusinessRule(
        "INVALID_RESET_CODE",
        "The reset code is invalid or has expired."));
}
