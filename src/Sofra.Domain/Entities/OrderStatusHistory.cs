using Sofra.Domain.Common;
using Sofra.Domain.Enums;

namespace Sofra.Domain.Entities;

/// <summary>
/// Immutable audit row — a weak entity owned by <see cref="Order"/>;
/// only created automatically when an order changes status.
/// </summary>
public class OrderStatusHistory : Entity
{
    public Guid OrderId { get; private set; }

    public Order Order { get; private set; } = null!;

    public OrderStatus Status { get; private set; }

    public Guid? ChangedByUserId { get; private set; }

    public AppUser? ChangedByUser { get; private set; }

    public string? Reason { get; private set; }

    public DateTime ChangedAt { get; private set; }

    private OrderStatusHistory() { }

    internal static OrderStatusHistory Create(Guid orderId, OrderStatus status, Guid changedBy, string? reason = null)
    {
        return new OrderStatusHistory
        {
            OrderId = orderId,
            Status = status,
            ChangedByUserId = changedBy,
            Reason = reason,
            ChangedAt = DateTime.UtcNow,
        };
    }
}
