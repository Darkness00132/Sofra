using MediatR;
using Microsoft.AspNetCore.Identity;
using Sofra.Application.Common.Interfaces;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Authentication.GetProfile;
using Sofra.Domain.Entities;

namespace Sofra.Application.Features.Authentication.UpdateProfile;

public class UpdateProfileCommandHandler(
    UserManager<AppUser> userManager,
    ICurrentUser currentUser) : IRequestHandler<UpdateProfileCommand, Result<UserProfileResponse>>
{
    public async Task<Result<UserProfileResponse>> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.GetUserId();

        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return Result<UserProfileResponse>.Failure(Error.NotFound(
                "USER_NOT_FOUND",
                "This user no longer exists."));

        user.PhoneNumber = request.PhoneNumber;
        var updateResult = await userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            var description = string.Join(" ", updateResult.Errors.Select(e => e.Description));
            return Result<UserProfileResponse>.Failure(Error.Validation("PROFILE_UPDATE_INVALID", description));
        }

        var roles = await userManager.GetRolesAsync(user);

        return new UserProfileResponse(user.Id, user.Email ?? string.Empty, user.PhoneNumber, roles.ToList());
    }
}
