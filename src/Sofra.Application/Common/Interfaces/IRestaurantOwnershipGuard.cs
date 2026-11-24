using Sofra.Application.Common.Results;
using Sofra.Domain.Entities;

namespace Sofra.Application.Common.Interfaces;

/// <summary>
/// Loads the restaurant owned by the current user, or the standard not-found failure.
/// </summary>
public interface IRestaurantOwnershipGuard
{
    Task<Result<Restaurant>> GetOwnedRestaurantAsync(Guid ownerUserId, CancellationToken cancellationToken = default);
}
