namespace Sofra.Application.Features.Orders.Dtos;

/// <summary>The placed order as returned right after checkout (FR-30).</summary>
public record PlaceOrderResponse(
    Guid Id,
    string Status,
    string FulfilmentType,
    string PaymentMethod,
    decimal Subtotal,
    decimal Tax,
    decimal DeliveryFee,
    decimal Total,
    string? DeliveryAddress,
    DateTime CreatedAt);
