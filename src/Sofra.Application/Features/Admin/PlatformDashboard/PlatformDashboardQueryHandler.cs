using MediatR;
using Sofra.Application.Common.Interfaces;
using Sofra.Application.Common.Results;
using Sofra.Application.Constants;

namespace Sofra.Application.Features.Admin.PlatformDashboard;

public class PlatformDashboardQueryHandler(IAccountRepository accounts, IRestaurantRepository restaurants, IRestaurantApplicationRepository applications, IOrderRepository orders) : IRequestHandler<PlatformDashboardQuery, Result<AdminDashboardResponse>>
{
    public async Task<Result<AdminDashboardResponse>> Handle(PlatformDashboardQuery request, CancellationToken cancellationToken)
    {
        var sales = await orders.GetPlatformSalesSummaryAsync(cancellationToken);
        var restaurantCounts = await restaurants.GetStatusCountsAsync(cancellationToken);
        var pendingApplications = await applications.CountPendingAsync(cancellationToken);
        var totalCustomers = await accounts.CountInRoleAsync(Roles.Customer, cancellationToken);
        return new AdminDashboardResponse(restaurantCounts.Total, restaurantCounts.Approved, restaurantCounts.Suspended, pendingApplications, totalCustomers, sales.TotalOrders, sales.PendingOrders, sales.TotalRevenue);
    }
}
