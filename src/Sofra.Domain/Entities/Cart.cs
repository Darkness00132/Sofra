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

    /// <summary>
    /// Creates a cart for a user. Carts are always scoped to one restaurant (SRS BR-02/FR-21);
    /// an empty cart clears its RestaurantId.
    /// </summary>
    public static Cart Create(Guid userId, Guid restaurantId)
    {
        return new Cart
        {
            UserId = userId,
            RestaurantId = restaurantId,
        };
    }

    public CartItem AddItem(CartItemSource source, int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Cart item quantity must be greater than zero.");

        if (!source.IsAvailable)
            throw new DomainException($"'{source.FoodItemName}' is unavailable and cannot be added to the cart.");

        EnsureSameRestaurant(source.RestaurantId);

        var existing = CartItems.FirstOrDefault(i => i.FoodItemId == source.FoodItemId);
        if (existing is not null)
        {
            existing.SetQuantity(existing.Quantity + quantity);
            return existing;
        }

        var item = CartItem.Create(Id, source.FoodItemId, quantity);
        CartItems.Add(item);
        return item;
    }

    public void RemoveItem(Guid foodItemId)
    {
        var item = CartItems.FirstOrDefault(i => i.FoodItemId == foodItemId);
        if (item is null)
            throw new DomainException("Item is not in the cart.");

        CartItems.Remove(item);

        if (CartItems.Count == 0)
        {
            RestaurantId = null;
            Restaurant = null;
        }
    }

    public void UpdateItemQuantity(Guid foodItemId, int quantity)
    {
        var item = CartItems.FirstOrDefault(i => i.FoodItemId == foodItemId);
        if (item is null)
            throw new DomainException("Item is not in the cart.");

        item.SetQuantity(quantity);
    }

    public void Clear()
    {
        CartItems.Clear();
        RestaurantId = null;
        Restaurant = null;
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
