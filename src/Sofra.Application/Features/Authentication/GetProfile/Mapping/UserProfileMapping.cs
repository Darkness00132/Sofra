using Mapster;
using Sofra.Domain.Entities;

namespace Sofra.Application.Features.Authentication.GetProfile.Mapping;

/// <summary>
/// AppUser maps to UserProfileResponse through same-name members (Id, Email, PhoneNumber) —
/// no configuration needed for those. Roles is deliberately ignored: the navigation
/// collection is not loaded, and the handler fills it from UserManager, where the async
/// role lookup belongs.
/// </summary>
public class UserProfileMapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<AppUser, UserProfileResponse>()
            .Ignore(d => d.Roles);
    }
}
