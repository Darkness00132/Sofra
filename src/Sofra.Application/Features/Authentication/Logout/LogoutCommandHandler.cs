using MediatR;
using Sofra.Application.Common.Interfaces;
using Sofra.Application.Common.Results;
using Sofra.Domain.Entities;

namespace Sofra.Application.Features.Authentication.Logout;

public class LogoutCommandHandler(IRepository<RefreshToken> refreshTokens, IUnitOfWork unitOfWork)
    : IRequestHandler<LogoutCommand, Result>
{
    public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var storedToken = await refreshTokens.SingleOrDefaultAsync(
            t => t.Token == request.RefreshToken, cancellationToken);

        if (storedToken is null)
            return Result.Failure(Error.Unauthorized(
                "INVALID_REFRESH_TOKEN",
                "The refresh token is invalid or already revoked."));

        if (storedToken.IsActive)
        {
            storedToken.Revoke();
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return Result.Success();
    }
}
