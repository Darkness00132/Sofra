using Sofra.Application.Common.Pagination;
using Sofra.Application.Features.Orders;
using Sofra.Application.Features.Orders.Dtos;
using Sofra.Domain.Entities;

namespace Sofra.Application.Common.Interfaces;

/// <summary>
/// Order repository — order details, paged lists per role, and the sales aggregates the
/// dashboards need all go beyond the generic repository's common operations.
/// </summary>
public interface IOrderRepository : IRepository<Order>
{
    /// <summary>Finds one order with its items (food details), status history, payments and restaurant loaded.</summary>
    Task<Order?> FindWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>One order's details, projected from the database — only the response columns come back.</summary>
    Task<OrderDetailsResponse?> GetDetailsAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Customer list: one customer's orders with filters and paging, newest first.</summary>
    Task<PagedResult<OrderSummaryResponse>> ListForCustomerAsync(Guid customerId, MyOrdersFilter filter, CancellationToken cancellationToken = default);

    /// <summary>Restaurant list: one restaurant's orders with filters and paging, newest first.</summary>
    Task<PagedResult<OrderSummaryResponse>> ListForRestaurantAsync(Guid restaurantId, RestaurantOrdersFilter filter, CancellationToken cancellationToken = default);

    /// <summary>Admin list: all orders with filters and paging, newest first.</summary>
    Task<PagedResult<OrderSummaryResponse>> ListForAdminAsync(AdminOrderListFilter filter, CancellationToken cancellationToken = default);

    /// <summary>Sales aggregates for one restaurant (owner dashboard).</summary>
    Task<OrderSalesSummary> GetRestaurantSalesSummaryAsync(Guid restaurantId, CancellationToken cancellationToken = default);

    /// <summary>Sales aggregates for the whole platform (admin dashboard).</summary>
    Task<OrderSalesSummary> GetPlatformSalesSummaryAsync(CancellationToken cancellationToken = default);

    /// <summary>True when past orders still reference this food item (BR-04 price snapshots).</summary>
    Task<bool> HasOrdersForFoodItemAsync(Guid foodItemId, CancellationToken cancellationToken = default);
}

/// <summary>Sales aggregates used by the dashboards.</summary>
public record OrderSalesSummary(
    int TotalOrders,
    int PendingOrders,
    int CompletedOrders,
    decimal TotalRevenue);
