using Sofra.Domain.Enums;

namespace Sofra.Application.Features.Admin.ListPayments;

/// <summary>Filter for the administrator's payment list.</summary>
public record PaymentListFilter(
    PaymentStatus? Status,
    PaymentMethod? Method,
    int PageNumber = 1,
    int PageSize = 10);
