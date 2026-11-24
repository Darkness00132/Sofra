namespace Sofra.Application.Features.Admin.PlatformDashboard;

/// <summary>Platform-wide overview for the administrator dashboard.</summary>
public record AdminDashboardResponse(
    int TotalRestaurants,
    int ApprovedRestaurants,
    int SuspendedRestaurants,
    int PendingApplications,
    int TotalCustomers,
    int TotalOrders,
    int PendingOrders,
    decimal TotalRevenue);
