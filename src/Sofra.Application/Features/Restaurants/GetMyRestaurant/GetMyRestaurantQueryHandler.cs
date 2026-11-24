using Mapster;
using MediatR;
using Sofra.Application.Common.Interfaces;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Restaurants.Dtos;
using Sofra.Domain.Entities;

namespace Sofra.Application.Features.Restaurants.GetMyRestaurant;

public class GetMyRestaurantQueryHandler(
    ICurrentUser currentUser,
    IRestaurantOwnershipGuard ownershipGuard) : IRequestHandler<GetMyRestaurantQuery, Result<MyRestaurantResponse>>
{
    public async Task<Result<MyRestaurantResponse>> Handle(GetMyRestaurantQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.GetUserId();

        var restaurantResult = await ownershipGuard.GetOwnedRestaurantAsync(userId, cancellationToken);
        if (restaurantResult.IsFailure)
            return Result<MyRestaurantResponse>.Failure(restaurantResult.Error!);

        var restaurant = restaurantResult.Response!;

        return restaurant.Adapt<MyRestaurantResponse>();
    }
}
