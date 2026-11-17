using MediatR;
using Sofra.Application.Common.Results;

namespace Sofra.Application.Features.Authentication.GetProfile;

/// <summary>Returns the profile of the currently signed-in user.</summary>
public record GetProfileQuery : IRequest<Result<UserProfileResponse>>;
