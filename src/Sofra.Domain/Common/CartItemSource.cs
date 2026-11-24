namespace Sofra.Domain.Common;

public record CartItemSource(
    Guid FoodItemId,
    Guid RestaurantId,
    string FoodItemName,
    bool IsAvailable);
