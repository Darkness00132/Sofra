using Sofra.Application.Common.Interfaces;
using Sofra.Application.Common.Results;
using Sofra.Domain.Entities;

namespace Sofra.Application.Services;

public class RestaurantOwnershipGuard(IRepository<Restaurant> restaurants) : IRestaurantOwnershipGuard
{
    public async Task<Result<Restaurant>> GetOwnedRestaurantAsync(Guid ownerUserId, CancellationToken cancellationToken = default)
    {
        var restaurant = await restaurants.FirstOrDefaultAsync(r => r.OwnerId == ownerUserId, cancellationToken);

        return restaurant is null
            ? Result<Restaurant>.Failure(Error.NotFound(
                "RESTAURANT_NOT_FOUND",
                "You do not have an approved restaurant yet."))
            : Result<Restaurant>.Success(restaurant);
    }
}
