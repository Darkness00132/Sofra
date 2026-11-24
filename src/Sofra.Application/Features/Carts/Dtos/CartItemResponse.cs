namespace Sofra.Application.Features.Carts.Dtos;

/// <summary>One item in the customer's cart.</summary>
public record CartItemResponse(
    Guid FoodItemId,
    string FoodItemName,
    decimal UnitPrice,
    int Quantity,
    decimal LineTotal);
