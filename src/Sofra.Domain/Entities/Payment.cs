using Sofra.Domain.Common;
using Sofra.Domain.Enums;
using Sofra.Domain.Exceptions;

namespace Sofra.Domain.Entities;

public class Payment : Entity
{
    public Guid OrderId { get; private set; }

    public Order Order { get; private set; } = null!;

    public decimal Amount { get; private set; }

    public PaymentMethod PaymentMethod { get; private set; }

    public PaymentStatus Status { get; private set; }

    public string? Metadata { get; private set; }

    public DateTime? PaidAt { get; private set; }

    public ICollection<Transaction> Transactions { get; private set; } = [];

    private Payment() { }

    public static Payment Create(Guid orderId, decimal amount, PaymentMethod paymentMethod)
    {
        if (amount <= 0)
            throw new DomainException("Payment amount must be greater than zero.");

        return new Payment
        {
            OrderId = orderId,
            Amount = amount,
            PaymentMethod = paymentMethod,
            Status = PaymentStatus.Pending,
        };
    }

    public void MarkCompleted(string? metadata = null)
    {
        EnsurePending(nameof(MarkCompleted));
        Status = PaymentStatus.Completed;
        PaidAt = DateTime.UtcNow;
        Metadata ??= metadata;
    }

    public void MarkFailed()
    {
        EnsurePending(nameof(MarkFailed));
        Status = PaymentStatus.Failed;
    }

    public void Refund()
    {
        if (Status != PaymentStatus.Completed)
            throw new DomainException($"Only a completed payment can be refunded, not {Status}.");

        Status = PaymentStatus.Refunded;
    }

    public void Cancel()
    {
        EnsurePending(nameof(Cancel));
        Status = PaymentStatus.Cancelled;
    }

    private void EnsurePending(string operation)
    {
        if (Status != PaymentStatus.Pending)
            throw new DomainException($"Payment is already {Status}; cannot {operation}.");
    }
}
