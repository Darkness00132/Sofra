using Sofra.Application.Features.Admin.ListPayments;
using Sofra.Domain.Entities;

namespace Sofra.Infrastructure.Repositories.Extensions;

public static class PaymentQueryableExtensions
{
    /// <summary>Applies the administrator's payment list filter, ordered newest first.</summary>
    public static IQueryable<Payment> ApplyFilter(this IQueryable<Payment> query, PaymentListFilter filter)
    {
        if (filter.Status is not null)
            query = query.Where(p => p.Status == filter.Status);

        if (filter.Method is not null)
            query = query.Where(p => p.PaymentMethod == filter.Method);

        return query.OrderByDescending(p => p.PaidAt);
    }
}
