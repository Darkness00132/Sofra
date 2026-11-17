using Sofra.Domain.Common;
using Sofra.Domain.Exceptions;

namespace Sofra.Domain.Entities;

/// <summary>Weak entity — owned by <see cref="Cart"/>; can only be created through <c>Cart.AddItem</c>.</summary>
public class CartItem : Entity
{
    public Guid CartId { get; private set; }

    public Cart Cart { get; private set; } = null!;

    public Guid FoodItemId { get; private set; }

    public FoodItem FoodItem { get; private set; } = null!;

    public int Quantity { get; private set; }

    private CartItem() { }

    internal static CartItem Create(Guid cartId, Guid foodItemId, int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Cart item quantity must be greater than zero.");

        return new CartItem
        {
            CartId = cartId,
            FoodItemId = foodItemId,
            Quantity = quantity,
        };
    }

    internal void SetQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Cart item quantity must be greater than zero.");

        Quantity = quantity;
    }
}
