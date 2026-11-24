using Sofra.Application.Features.Restaurants.Admin.ListApplications;
using Sofra.Domain.Entities;

namespace Sofra.Infrastructure.Repositories.Extensions;

public static class RestaurantApplicationQueryableExtensions
{
    /// <summary>Applies the administrator's application list filter, ordered newest first.</summary>
    public static IQueryable<RestaurantApplication> ApplyFilter(this IQueryable<RestaurantApplication> query, ApplicationListFilter filter)
    {
        if (!string.IsNullOrWhiteSpace(filter.Search))
            query = query.Where(a => a.RestaurantName.Contains(filter.Search));

        if (filter.Status is not null)
            query = query.Where(a => a.Status == filter.Status);

        return query.OrderByDescending(a => a.CreatedAt);
    }
}
