using Sofra.Domain.Entities;
using Sofra.Domain.Enums;

namespace Sofra.Infrastructure.Repositories.Extensions;

public static class OrderQueryableExtensions
{
    /// <summary>Applies the shared status/date window filters, ordered newest first.</summary>
    public static IQueryable<Order> ApplyFilter(this IQueryable<Order> query, OrderStatus? status, DateTime? from, DateTime? to)
    {
        if (status is not null)
            query = query.Where(o => o.Status == status);

        if (from is not null)
            query = query.Where(o => o.CreatedAt >= from);

        if (to is not null)
            query = query.Where(o => o.CreatedAt <= to);

        return query.OrderByDescending(o => o.CreatedAt);
    }
}
