using Microsoft.EntityFrameworkCore;
using Sofra.Application.Common.Pagination;

namespace Sofra.Infrastructure.Repositories;

/// <summary>
/// Extensions for applying list features (paging, filters) to queries. Filter records are
/// defined in the feature folder; per-entity <c>ApplyFilter</c> extensions live here next to
/// their repositories and compose with these shared helpers.
/// </summary>
public static class QueryableExtensions
{
    /// <summary>Slices an already-shaped query into one page.</summary>
    public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, int pageNumber, int pageSize) =>
        query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);

    /// <summary>Counts and slices a query into the standard paged result.</summary>
    public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
        this IQueryable<T> query, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.ApplyPaging(pageNumber, pageSize).ToListAsync(cancellationToken);

        return new PagedResult<T>(items, pageNumber, pageSize, totalCount);
    }
}
