using MediatR;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Authentication.GetProfile;

namespace Sofra.Application.Features.Authentication.UpdateProfile;

/// <summary>Updates the signed-in user's profile information (FR-03).</summary>
public record UpdateProfileCommand(string PhoneNumber) : IRequest<Result<UserProfileResponse>>;
