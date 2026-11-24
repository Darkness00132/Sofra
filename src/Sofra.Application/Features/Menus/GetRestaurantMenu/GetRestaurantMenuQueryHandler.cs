using MediatR;
using Sofra.Application.Common.Interfaces;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Menus.Dtos;
using Sofra.Domain.Entities;
using Sofra.Domain.Enums;

namespace Sofra.Application.Features.Menus.GetRestaurantMenu;

public class GetRestaurantMenuQueryHandler(
    IRepository<Restaurant> restaurants,
    IMenuRepository menus) : IRequestHandler<GetRestaurantMenuQuery, Result<RestaurantMenuResponse>>
{
    public async Task<Result<RestaurantMenuResponse>> Handle(GetRestaurantMenuQuery request, CancellationToken cancellationToken)
    {
        var restaurant = await restaurants.FindAsync(request.RestaurantId, cancellationToken);
        if (restaurant is null || restaurant.Status != RestaurantStatus.Approved)
            return Result<RestaurantMenuResponse>.Failure(Error.NotFound(
                "RESTAURANT_NOT_FOUND",
                "The restaurant was not found."));

        var types = await menus.GetPublicMenuTypesAsync(restaurant.Id, cancellationToken);

        return Result<RestaurantMenuResponse>.Success(
            new RestaurantMenuResponse(restaurant.Id, restaurant.Name, types));
    }
}
