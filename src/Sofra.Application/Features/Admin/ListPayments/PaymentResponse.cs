namespace Sofra.Application.Features.Admin.ListPayments;

/// <summary>A payment record as seen by administrators.</summary>
public record PaymentResponse(
    Guid Id,
    Guid OrderId,
    decimal Amount,
    string PaymentMethod,
    string Status,
    DateTime? PaidAt);
