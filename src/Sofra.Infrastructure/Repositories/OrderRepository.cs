using Mapster;
using Microsoft.EntityFrameworkCore;
using Sofra.Application.Common.Interfaces;
using Sofra.Application.Common.Pagination;
using Sofra.Application.Features.Orders;
using Sofra.Application.Features.Orders.Dtos;
using Sofra.Domain.Entities;
using Sofra.Domain.Enums;
using Sofra.Infrastructure.Repositories.Extensions;
using Sofra.Infrastructure.Persistent;

namespace Sofra.Infrastructure.Repositories;

public class OrderRepository(SofraDbContext context)
    : Repository<Order>(context), IOrderRepository
{
    public Task<Order?> FindWithDetailsAsync(Guid id, CancellationToken cancellationToken = default) =>
        Context.Orders
            .Include(o => o.Restaurant)
            .Include(o => o.OrderItems).ThenInclude(i => i.FoodItem)
            .Include(o => o.StatusHistory)
            .Include(o => o.Payments)
            .AsSplitQuery()
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

    public Task<OrderDetailsResponse?> GetDetailsAsync(Guid id, CancellationToken cancellationToken = default) =>
        Context.Orders
            .Where(o => o.Id == id)
            .ProjectToType<OrderDetailsResponse>(TypeAdapterConfig.GlobalSettings)
            .FirstOrDefaultAsync(cancellationToken);

    public Task<bool> HasOrdersForFoodItemAsync(Guid foodItemId, CancellationToken cancellationToken = default) =>
        Context.OrderItems.AnyAsync(i => i.FoodItemId == foodItemId, cancellationToken);

    public async Task<PagedResult<OrderSummaryResponse>> ListForCustomerAsync(Guid customerId, MyOrdersFilter filter, CancellationToken cancellationToken = default)
    {
        var query = Context.Orders
            .Where(o => o.CustomerId == customerId)
            .ApplyFilter(filter.Status, filter.From, filter.To)
            .ProjectToType<OrderSummaryResponse>(TypeAdapterConfig.GlobalSettings);

        return await this.ToPagedResultAsync(query, filter.PageNumber, filter.PageSize, cancellationToken);
    }

    public async Task<PagedResult<OrderSummaryResponse>> ListForRestaurantAsync(Guid restaurantId, RestaurantOrdersFilter filter, CancellationToken cancellationToken = default)
    {
        var query = Context.Orders
            .Where(o => o.RestaurantId == restaurantId)
            .ApplyFilter(filter.Status, null, null)
            .ProjectToType<OrderSummaryResponse>(TypeAdapterConfig.GlobalSettings);

        return await this.ToPagedResultAsync(query, filter.PageNumber, filter.PageSize, cancellationToken);
    }

    public async Task<PagedResult<OrderSummaryResponse>> ListForAdminAsync(AdminOrderListFilter filter, CancellationToken cancellationToken = default)
    {
        var query = Context.Orders
            .ApplyFilter(filter.Status, filter.From, filter.To);

        if (filter.RestaurantId is not null)
            query = query.Where(o => o.RestaurantId == filter.RestaurantId);

        if (filter.CustomerId is not null)
            query = query.Where(o => o.CustomerId == filter.CustomerId);

        var projected = query.ProjectToType<OrderSummaryResponse>(TypeAdapterConfig.GlobalSettings);

        return await this.ToPagedResultAsync(projected, filter.PageNumber, filter.PageSize, cancellationToken);
    }

    public async Task<OrderSalesSummary> GetRestaurantSalesSummaryAsync(Guid restaurantId, CancellationToken cancellationToken = default)
    {
        // Conditional aggregates over one group: one round trip, zero-summary when there are no orders.
        var summary = await Context.Orders
            .Where(o => o.RestaurantId == restaurantId)
            .GroupBy(_ => 1)
            .OrderBy(g => g.Key)
            .Select(g => new
            {
                TotalOrders = g.Count(),
                PendingOrders = g.Count(o => o.Status == OrderStatus.Pending),
                CompletedOrders = g.Count(o => o.Status == OrderStatus.Completed),
                TotalRevenue = g.Where(o => o.Status == OrderStatus.Completed)
                    .Sum(o => (decimal?)o.Total) ?? 0m,
            })
            .FirstOrDefaultAsync(cancellationToken);

        return summary is null ? new OrderSalesSummary(0, 0, 0, 0m) : new OrderSalesSummary(summary.TotalOrders, summary.PendingOrders, summary.CompletedOrders, summary.TotalRevenue);
    }

    public async Task<OrderSalesSummary> GetPlatformSalesSummaryAsync(CancellationToken cancellationToken = default)
    {
        // Conditional aggregates over one group: one round trip, zero-summary when there are no orders.
        var summary = await Context.Orders
            .GroupBy(_ => 1)
            .OrderBy(g => g.Key)
            .Select(g => new
            {
                TotalOrders = g.Count(),
                PendingOrders = g.Count(o => o.Status == OrderStatus.Pending),
                CompletedOrders = g.Count(o => o.Status == OrderStatus.Completed),
                TotalRevenue = g.Where(o => o.Status == OrderStatus.Completed)
                    .Sum(o => (decimal?)o.Total) ?? 0m,
            })
            .FirstOrDefaultAsync(cancellationToken);

        return summary is null ? new OrderSalesSummary(0, 0, 0, 0m) : new OrderSalesSummary(summary.TotalOrders, summary.PendingOrders, summary.CompletedOrders, summary.TotalRevenue);
    }
}
