using MediatR;
using Sofra.Application.Common.Interfaces;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Orders.Dtos;
using Sofra.Domain.Entities;

namespace Sofra.Application.Features.Orders.GetRestaurantOrder;

public class GetRestaurantOrderQueryHandler(
    ICurrentUser currentUser,
    IRestaurantOwnershipGuard ownershipGuard,
    IOrderRepository orders) : IRequestHandler<GetRestaurantOrderQuery, Result<OrderDetailsResponse>>
{
    public async Task<Result<OrderDetailsResponse>> Handle(GetRestaurantOrderQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.GetUserId();

        var restaurantResult = await ownershipGuard.GetOwnedRestaurantAsync(userId, cancellationToken);
        if (restaurantResult.IsFailure)
            return Result<OrderDetailsResponse>.Failure(restaurantResult.Error!);

        var restaurant = restaurantResult.Response!;

        var order = await orders.GetDetailsAsync(request.OrderId, cancellationToken);
        if (order is null || order.RestaurantId != restaurant.Id)
            return Result<OrderDetailsResponse>.Failure(Error.NotFound(
                "ORDER_NOT_FOUND",
                "The order was not found."));

        return order;
    }
}
