namespace Sofra.Application.Features.Orders.Dtos;

/// <summary>One recorded status change of an order (FR-36 tracking).</summary>
public record OrderStatusHistoryResponse(
    string Status,
    Guid? ChangedByUserId,
    string? Reason,
    DateTime ChangedAt);
