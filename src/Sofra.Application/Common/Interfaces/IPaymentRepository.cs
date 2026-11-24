using Sofra.Application.Common.Pagination;
using Sofra.Application.Features.Admin.ListPayments;
using Sofra.Domain.Entities;

namespace Sofra.Application.Common.Interfaces;

/// <summary>
/// Payment repository — the paged, filtered administrator list goes beyond the generic
/// repository's common operations.
/// </summary>
public interface IPaymentRepository : IRepository<Payment>
{
    /// <summary>Admin list: applies the filter, ordered newest first by payment time.</summary>
    Task<PagedResult<PaymentResponse>> ListAsync(PaymentListFilter filter, CancellationToken cancellationToken = default);
}
