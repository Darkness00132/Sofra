using Mapster;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Sofra.Application.Common.Interfaces;
using Sofra.Application.Common.Results;
using Sofra.Domain.Entities;
using Sofra.Application.Constants;

namespace Sofra.Application.Features.Authentication.GetProfile;

public class GetProfileQueryHandler(
    UserManager<AppUser> userManager,
    ICurrentUser currentUser) : IRequestHandler<GetProfileQuery, Result<UserProfileResponse>>
{
    public async Task<Result<UserProfileResponse>> Handle(GetProfileQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.GetUserId();

        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return Result<UserProfileResponse>.Failure(Error.NotFound(
                "USER_NOT_FOUND",
                "This user no longer exists."));

        var roles = await userManager.GetRolesAsync(user);
        var response = user.Adapt<UserProfileResponse>() with { Roles = roles.ToList() };

        return response;
    }
}
