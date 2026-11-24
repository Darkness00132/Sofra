using MediatR;
using Sofra.Application.Common.Interfaces;
using Sofra.Application.Common.Pagination;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Orders.Dtos;
using Sofra.Domain.Entities;

namespace Sofra.Application.Features.Orders.ListRestaurantOrders;

public class ListRestaurantOrdersQueryHandler(
    ICurrentUser currentUser,
    IRestaurantOwnershipGuard ownershipGuard,
    IOrderRepository orders) : IRequestHandler<ListRestaurantOrdersQuery, Result<PagedResult<OrderSummaryResponse>>>
{
    public async Task<Result<PagedResult<OrderSummaryResponse>>> Handle(ListRestaurantOrdersQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.GetUserId();

        var restaurantResult = await ownershipGuard.GetOwnedRestaurantAsync(userId, cancellationToken);
        if (restaurantResult.IsFailure)
            return Result<PagedResult<OrderSummaryResponse>>.Failure(restaurantResult.Error!);

        var restaurant = restaurantResult.Response!;

        return await orders.ListForRestaurantAsync(restaurant.Id, request.Filter, cancellationToken);
    }
}
