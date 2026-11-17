using System.Security.Cryptography;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Sofra.Application.Common.Interfaces;
using Sofra.Application.Common.Results;
using Sofra.Domain.Entities;

namespace Sofra.Application.Features.Authentication.ForgotPassword;

public class ForgotPasswordCommandHandler(
    UserManager<AppUser> userManager,
    IRepository<PasswordResetOtp> resetCodes,
    IUnitOfWork unitOfWork,
    IEmailSender emailSender) : IRequestHandler<ForgotPasswordCommand, Result>
{
    private const int CodeLifetimeMinutes = 10;

    public async Task<Result> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is not null)
        {
            // a new code consumes every still-active one
            var activeCodes = await resetCodes.ListAsync(
                o => o.UserId == user.Id && o.UsedAt == null && o.ExpiresAt > DateTime.UtcNow, cancellationToken);
            foreach (var activeCode in activeCodes)
                activeCode.MarkUsed();

            var code = RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");
            await resetCodes.AddAsync(
                PasswordResetOtp.Create(
                    user.Id,
                    PasswordResetOtp.Hash(code),
                    DateTime.UtcNow.AddMinutes(CodeLifetimeMinutes)),
                cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            await emailSender.SendAsync(
                user.Email!,
                "Sofra password reset code",
                $"<p>Your password reset code is <strong>{code}</strong>. It expires in {CodeLifetimeMinutes} minutes.</p>" +
                $"<p>If you did not request a password reset, you can ignore this email.</p>",
                cancellationToken);
        }

        return Result.Success();
    }
}
