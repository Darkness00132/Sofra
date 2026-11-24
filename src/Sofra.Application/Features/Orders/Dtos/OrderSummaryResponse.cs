namespace Sofra.Application.Features.Orders.Dtos;

/// <summary>Compact order row for lists (FR-36, FR-43, FR-44).</summary>
public record OrderSummaryResponse(
    Guid Id,
    Guid RestaurantId,
    string RestaurantName,
    string Status,
    string FulfilmentType,
    string DeliveryStatus,
    string PaymentMethod,
    decimal Total,
    DateTime CreatedAt);
