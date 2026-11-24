namespace Sofra.Application.Features.Orders.Dtos;

/// <summary>Full details of one order — items, totals, payment, delivery and status history.</summary>
public record OrderDetailsResponse(
    Guid Id,
    Guid CustomerId,
    Guid RestaurantId,
    string RestaurantName,
    string Status,
    string FulfilmentType,
    string DeliveryStatus,
    string PaymentMethod,
    string? PaymentStatus,
    decimal Subtotal,
    decimal Tax,
    decimal DeliveryFee,
    decimal Discount,
    decimal Total,
    string? DeliveryAddress,
    DateTime CreatedAt,
    DateTime? CompletedAt,
    IReadOnlyList<OrderItemResponse> Items,
    IReadOnlyList<OrderStatusHistoryResponse> StatusHistory);
