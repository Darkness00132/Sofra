using Sofra.Domain.Common;
using Sofra.Domain.Exceptions;

namespace Sofra.Domain.Entities;

/// <summary>A customer's cart. Holds items from a single restaurant (SRS rule).</summary>
public class Cart : Entity
{
    public Guid UserId { get; private set; }

    public AppUser User { get; private set; } = null!;

    public Guid? RestaurantId { get; private set; }

    public Restaurant? Restaurant { get; private set; }

    public ICollection<CartItem> CartItems { get; private set; } = [];

    private Cart() { }

    public static Cart CreateForRestaurant(Guid userId, Guid restaurantId)
    {
        return new Cart
        {
            UserId = userId,
            RestaurantId = restaurantId,
        };
    }

    /// <summary>Adding the same food item again increases its quantity instead of duplicating the row.</summary>
    public CartItem AddItem(FoodItem foodItem, int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Cart item quantity must be greater than zero.");

        if (!foodItem.IsAvailable)
            throw new DomainException($"'{foodItem.Name}' is unavailable and cannot be added to the cart.");

        EnsureSameRestaurant(foodItem.FoodType.RestaurantId);

        var existing = CartItems.FirstOrDefault(i => i.FoodItemId == foodItem.Id);
        if (existing is not null)
        {
            existing.SetQuantity(existing.Quantity + quantity);
            return existing;
        }

        var item = CartItem.Create(Id, foodItem.Id, quantity);
        CartItems.Add(item);
        return item;
    }

    public void RemoveItem(Guid foodItemId)
    {
        var item = CartItems.FirstOrDefault(i => i.FoodItemId == foodItemId);
        if (item is null)
            throw new DomainException("Item is not in the cart.");

        CartItems.Remove(item);

        // an empty cart is not tied to a restaurant anymore
        if (CartItems.Count == 0)
            RestaurantId = null;
    }

    public bool IsEmpty => CartItems.Count == 0;

    private void EnsureSameRestaurant(Guid restaurantId)
    {
        if (RestaurantId is null)
        {
            RestaurantId = restaurantId;
            return;
        }

        if (RestaurantId != restaurantId)
            throw new DomainException("A cart can only hold items from one restaurant. Clear the cart first.");
    }
}
