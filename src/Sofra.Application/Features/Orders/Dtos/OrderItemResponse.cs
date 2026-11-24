namespace Sofra.Application.Features.Orders.Dtos;

/// <summary>One ordered item — the price snapshot at order time (BR-04).</summary>
public record OrderItemResponse(
    Guid FoodItemId,
    string FoodItemName,
    decimal UnitPrice,
    int Quantity,
    decimal LineTotal);
