using Sofra.Domain.Common;
using Sofra.Domain.Exceptions;

namespace Sofra.Domain.Entities;

/// <summary>Weak entity — owned by <see cref="Order"/>; can only be created through <c>Order.AddItem</c>.</summary>
public class OrderItem : Entity
{
    public Guid OrderId { get; private set; }

    public Order Order { get; private set; } = null!;

    public Guid FoodItemId { get; private set; }

    public FoodItem FoodItem { get; private set; } = null!;

    public int Quantity { get; private set; }

    /// <summary>Unit price at the time the order was placed (price snapshot).</summary>
    public decimal Price { get; private set; }

    private OrderItem() { }

    internal static OrderItem Create(Guid orderId, Guid foodItemId, int quantity, decimal unitPrice)
    {
        if (quantity <= 0)
            throw new DomainException("Order item quantity must be greater than zero.");

        if (unitPrice < 0)
            throw new DomainException("Order item price cannot be negative.");

        return new OrderItem
        {
            OrderId = orderId,
            FoodItemId = foodItemId,
            Quantity = quantity,
            Price = unitPrice,
        };
    }

    internal void SetQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Order item quantity must be greater than zero.");

        Quantity = quantity;
    }

    public decimal LineTotal => Quantity * Price;
}
