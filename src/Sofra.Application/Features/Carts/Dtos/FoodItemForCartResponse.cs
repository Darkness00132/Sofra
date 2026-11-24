namespace Sofra.Application.Features.Carts.Dtos;

public record FoodItemForCartResponse(
    Guid Id,
    string Name,
    bool IsAvailable,
    Guid RestaurantId);
