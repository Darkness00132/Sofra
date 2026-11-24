using MediatR;
using Sofra.Application.Common.Interfaces;
using Sofra.Application.Common.Results;
using Sofra.Domain.Entities;

namespace Sofra.Application.Features.Orders.RestaurantDashboard;

public class RestaurantDashboardQueryHandler(
    ICurrentUser currentUser,
    IRestaurantOwnershipGuard ownershipGuard,
    IOrderRepository orders) : IRequestHandler<RestaurantDashboardQuery, Result<OwnerDashboardResponse>>
{
    public async Task<Result<OwnerDashboardResponse>> Handle(RestaurantDashboardQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.GetUserId();

        var restaurantResult = await ownershipGuard.GetOwnedRestaurantAsync(userId, cancellationToken);
        if (restaurantResult.IsFailure)
            return Result<OwnerDashboardResponse>.Failure(restaurantResult.Error!);

        var restaurant = restaurantResult.Response!;

        var summary = await orders.GetRestaurantSalesSummaryAsync(restaurant.Id, cancellationToken);

        return new OwnerDashboardResponse(
            restaurant.Id,
            restaurant.Name,
            summary.TotalOrders,
            summary.PendingOrders,
            summary.CompletedOrders,
            summary.TotalRevenue);
    }
}
