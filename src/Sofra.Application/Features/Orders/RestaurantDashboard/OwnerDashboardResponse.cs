namespace Sofra.Application.Features.Orders.RestaurantDashboard;

/// <summary>Basic sales information for the owner's restaurant.</summary>
public record OwnerDashboardResponse(
    Guid RestaurantId,
    string RestaurantName,
    int TotalOrders,
    int PendingOrders,
    int CompletedOrders,
    decimal TotalRevenue);
