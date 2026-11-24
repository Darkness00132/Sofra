using Sofra.Application.Features.Discovery;
using Sofra.Application.Features.Restaurants.Admin.ListRestaurants;
using Sofra.Domain.Entities;

namespace Sofra.Infrastructure.Repositories.Extensions;

public static class RestaurantQueryableExtensions
{
    /// <summary>Applies the administrator's restaurant list filter, ordered by name.</summary>
    public static IQueryable<Restaurant> ApplyFilter(this IQueryable<Restaurant> query, RestaurantListFilter filter)
    {
        if (!string.IsNullOrWhiteSpace(filter.Search))
            query = query.Where(r => r.Name.Contains(filter.Search));

        if (filter.Status is not null)
            query = query.Where(r => r.Status == filter.Status);

        return query.OrderBy(r => r.Name);
    }

    /// <summary>Applies the public discovery filter (FR-06, FR-08, FR-09), ordered by name.</summary>
    public static IQueryable<Restaurant> ApplyFilter(this IQueryable<Restaurant> query, PublicRestaurantFilter filter)
    {
        if (!string.IsNullOrWhiteSpace(filter.Search))
            query = query.Where(r => r.Name.Contains(filter.Search));

        if (filter.CategoryId is not null)
            query = query.Where(r => r.FoodTypes.Any(t => t.FoodCategoryId == filter.CategoryId));

        if (filter.AcceptingOrders is not null)
            query = query.Where(r => r.IsAcceptingOrders == filter.AcceptingOrders);

        return query.OrderBy(r => r.Name);
    }
}
