using Sofra.Application.Common.Pagination;
using Sofra.Application.Features.FoodCategories;
using Sofra.Application.Features.FoodCategories.Dtos;
using Sofra.Domain.Entities;

namespace Sofra.Application.Common.Interfaces;

/// <summary>Food-category repository — the public catalog list needs paging and search.</summary>
public interface IFoodCategoryRepository : IRepository<FoodCategory>
{
    /// <summary>Public catalog list: applies the filter, ordered by name.</summary>
    Task<PagedResult<FoodCategoryResponse>> ListAsync(FoodCategoryFilter filter, CancellationToken cancellationToken = default);
}
