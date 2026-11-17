using Sofra.Domain.Common;
using Sofra.Domain.Enums;
using Sofra.Domain.Exceptions;

namespace Sofra.Domain.Entities;

public class Transaction : Entity
{
    public Guid? OrderId { get; private set; }

    public Order? Order { get; private set; }

    public Guid? PaymentId { get; private set; }

    public Payment? Payment { get; private set; }

    public TransactionType Type { get; private set; }

    public decimal Amount { get; private set; }

    public string? Description { get; private set; }

    public string? ReferenceNumber { get; private set; }

    public TransactionStatus Status { get; private set; }

    public Guid? RestaurantId { get; private set; }

    public Restaurant? Restaurant { get; private set; }

    public DateTime CreatedAt { get; private set; }

    private Transaction() { }

    public static Transaction Create(
        TransactionType type,
        decimal amount,
        Guid? restaurantId = null,
        Guid? paymentId = null,
        Guid? orderId = null,
        string? description = null,
        string? referenceNumber = null)
    {
        if (amount <= 0)
            throw new DomainException("Transaction amount must be greater than zero.");

        return new Transaction
        {
            Type = type,
            Amount = amount,
            RestaurantId = restaurantId,
            PaymentId = paymentId,
            OrderId = orderId,
            Description = description,
            ReferenceNumber = referenceNumber,
            Status = TransactionStatus.Completed,
            CreatedAt = DateTime.UtcNow,
        };
    }

    public void MarkFailed()
    {
        EnsureNotTerminal(nameof(MarkFailed));
        Status = TransactionStatus.Failed;
    }

    public void Reverse()
    {
        if (Status != TransactionStatus.Completed)
            throw new DomainException($"Only a completed transaction can be reversed, not {Status}.");

        Status = TransactionStatus.Reversed;
    }

    private void EnsureNotTerminal(string operation)
    {
        if (Status is TransactionStatus.Reversed or TransactionStatus.Failed)
            throw new DomainException($"Transaction is {Status}; cannot {operation}.");
    }
}
