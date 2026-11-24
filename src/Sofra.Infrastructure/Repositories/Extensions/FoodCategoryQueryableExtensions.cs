using Sofra.Application.Features.FoodCategories;
using Sofra.Domain.Entities;

namespace Sofra.Infrastructure.Repositories.Extensions;

public static class FoodCategoryQueryableExtensions
{
    /// <summary>Applies the public catalog filter, ordered by name.</summary>
    public static IQueryable<FoodCategory> ApplyFilter(this IQueryable<FoodCategory> query, FoodCategoryFilter filter)
    {
        if (!string.IsNullOrWhiteSpace(filter.Search))
            query = query.Where(c => c.Name.Contains(filter.Search));

        return query.OrderBy(c => c.Name);
    }
}
