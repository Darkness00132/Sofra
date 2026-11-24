namespace Sofra.Application.Features.Orders.PreviewOrder;

/// <summary>What the checkout would charge for the current cart.</summary>
public record OrderPreviewResponse(
    Guid RestaurantId,
    string RestaurantName,
    decimal Subtotal,
    decimal Tax,
    decimal DeliveryFee,
    decimal Total);
