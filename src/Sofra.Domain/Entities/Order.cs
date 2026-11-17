using Sofra.Domain.Common;
using Sofra.Domain.Enums;
using Sofra.Domain.Exceptions;

namespace Sofra.Domain.Entities;

public class Order : Entity
{
    public Guid CustomerId { get; private set; }

    public AppUser Customer { get; private set; } = null!;

    public Guid RestaurantId { get; private set; }

    public Restaurant Restaurant { get; private set; } = null!;

    public FulfilmentType FulfilmentType { get; private set; }

    public PaymentMethod PaymentMethod { get; private set; }

    public OrderStatus Status { get; private set; }

    public DeliveryStatus DeliveryStatus { get; private set; }

    public decimal Subtotal { get; private set; }

    public decimal Tax { get; private set; }

    public decimal DeliveryFee { get; private set; }

    public decimal Discount { get; private set; }

    public decimal Total { get; private set; }

    public string? DeliveryAddress { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? CompletedAt { get; private set; }

    public ICollection<OrderItem> OrderItems { get; private set; } = [];
    public ICollection<OrderStatusHistory> StatusHistory { get; private set; } = [];
    public ICollection<Payment> Payments { get; private set; } = [];
    public ICollection<Transaction> Transactions { get; private set; } = [];

    private Order() { }

    public static Order Create(
        Guid customerId,
        Guid restaurantId,
        FulfilmentType fulfilmentType,
        PaymentMethod paymentMethod,
        string? deliveryAddress = null)
    {
        if (fulfilmentType == FulfilmentType.Delivery && string.IsNullOrWhiteSpace(deliveryAddress))
            throw new DomainException("Delivery address is required for delivery orders.");

        return new Order
        {
            CustomerId = customerId,
            RestaurantId = restaurantId,
            FulfilmentType = fulfilmentType,
            PaymentMethod = paymentMethod,
            Status = OrderStatus.Pending,
            DeliveryStatus = fulfilmentType == FulfilmentType.Delivery
                ? DeliveryStatus.Pending
                : DeliveryStatus.NotApplicable,
            DeliveryAddress = deliveryAddress,
            CreatedAt = DateTime.UtcNow,
        };
    }

    public OrderItem AddItem(Guid foodItemId, int quantity, decimal unitPrice)
    {
        EnsureStatusIsPending("add items to");

        var item = OrderItem.Create(Id, foodItemId, quantity, unitPrice);
        OrderItems.Add(item);
        return item;
    }

    public void RemoveItem(Guid orderItemId)
    {
        EnsureStatusIsPending("remove items from");

        var item = OrderItems.FirstOrDefault(i => i.Id == orderItemId);
        if (item is null)
            throw new DomainException("Order item was not found on this order.");

        OrderItems.Remove(item);
    }

    public void SetTotals(decimal tax, decimal deliveryFee, decimal discount)
    {
        EnsureStatusIsPending("set totals on");

        if (OrderItems.Count == 0)
            throw new DomainException("Cannot set totals on an empty order.");

        if (tax < 0 || deliveryFee < 0 || discount < 0)
            throw new DomainException("Order totals cannot be negative.");

        var subtotal = OrderItems.Sum(i => i.LineTotal);
        if (discount > subtotal + tax + deliveryFee)
            throw new DomainException("Discount cannot exceed the order amount.");

        Subtotal = subtotal;
        Tax = tax;
        DeliveryFee = deliveryFee;
        Discount = discount;
        Total = subtotal + tax + deliveryFee - discount;
    }

    public void Accept(Guid changedBy)
    {
        if (Status != OrderStatus.Pending)
            throw new DomainException($"Order can only be accepted from Pending, not {Status}.");

        Status = OrderStatus.Accepted;
        AddStatusHistory(changedBy);
    }

    public void StartPreparing(Guid changedBy)
    {
        if (Status != OrderStatus.Accepted)
            throw new DomainException($"Order can only start preparing from Accepted, not {Status}.");

        Status = OrderStatus.Preparing;
        AddStatusHistory(changedBy);
    }

    public void MarkReady(Guid changedBy)
    {
        if (Status != OrderStatus.Preparing)
            throw new DomainException($"Order can only be marked ready from Preparing, not {Status}.");

        Status = OrderStatus.Ready;
        if (FulfilmentType == FulfilmentType.Delivery)
            DeliveryStatus = DeliveryStatus.Collected;

        AddStatusHistory(changedBy);
    }

    public void Complete(Guid changedBy)
    {
        if (Status != OrderStatus.Ready)
            throw new DomainException($"Order can only be completed from Ready, not {Status}.");

        Status = OrderStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        if (FulfilmentType == FulfilmentType.Delivery)
            DeliveryStatus = DeliveryStatus.Delivered;

        AddStatusHistory(changedBy);
    }

    public void Reject(string? reason, Guid changedBy)
    {
        if (Status != OrderStatus.Pending)
            throw new DomainException($"Order can only be rejected from Pending, not {Status}.");

        Status = OrderStatus.Rejected;
        AddStatusHistory(changedBy, reason);
    }

    public void Cancel(OrderCancellationReason reason, string? note, Guid changedBy)
    {
        if (Status is OrderStatus.Completed or OrderStatus.Rejected or OrderStatus.Cancelled)
            throw new DomainException($"Order in {Status} state cannot be cancelled.");

        Status = OrderStatus.Cancelled;
        AddStatusHistory(changedBy, note);
    }

    private void EnsureStatusIsPending(string operation)
    {
        if (Status != OrderStatus.Pending)
            throw new DomainException($"Cannot {operation} an order that is no longer in Pending state.");
    }

    private void AddStatusHistory(Guid changedBy, string? reason = null)
    {
        StatusHistory.Add(OrderStatusHistory.Create(Id, Status, changedBy, reason));
    }
}
