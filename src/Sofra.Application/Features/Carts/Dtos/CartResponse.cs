namespace Sofra.Application.Features.Carts.Dtos;

/// <summary>The customer's cart — one restaurant's items with their subtotal (FR-23).</summary>
public record CartResponse(
    Guid? RestaurantId,
    string? RestaurantName,
    IReadOnlyList<CartItemResponse> Items,
    decimal Subtotal);
