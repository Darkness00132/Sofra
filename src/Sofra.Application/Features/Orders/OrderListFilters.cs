using Sofra.Domain.Enums;

namespace Sofra.Application.Features.Orders;

/// <summary>Filter for a customer's own order list.</summary>
public record MyOrdersFilter(
    OrderStatus? Status,
    DateTime? From,
    DateTime? To,
    int PageNumber = 1,
    int PageSize = 10);

/// <summary>Filter for a restaurant's order list.</summary>
public record RestaurantOrdersFilter(
    OrderStatus? Status,
    int PageNumber = 1,
    int PageSize = 10);

/// <summary>Filter for the administrator's platform-wide order list.</summary>
public record AdminOrderListFilter(
    Guid? RestaurantId,
    Guid? CustomerId,
    OrderStatus? Status,
    DateTime? From,
    DateTime? To,
    int PageNumber = 1,
    int PageSize = 10);
