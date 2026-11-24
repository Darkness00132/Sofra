using Mapster;
using Microsoft.EntityFrameworkCore;
using Sofra.Application.Common.Interfaces;
using Sofra.Application.Common.Pagination;
using Sofra.Application.Features.FoodCategories;
using Sofra.Application.Features.FoodCategories.Dtos;
using Sofra.Domain.Entities;
using Sofra.Infrastructure.Repositories.Extensions;
using Sofra.Infrastructure.Persistent;

namespace Sofra.Infrastructure.Repositories;

public class FoodCategoryRepository(SofraDbContext context) : Repository<FoodCategory>(context), IFoodCategoryRepository
{
    public async Task<PagedResult<FoodCategoryResponse>> ListAsync(FoodCategoryFilter filter, CancellationToken cancellationToken = default)
    {
        var query = Context.FoodCategories.ApplyFilter(filter).ProjectToType<FoodCategoryResponse>(TypeAdapterConfig.GlobalSettings);
        return await this.ToPagedResultAsync(query, filter.PageNumber, filter.PageSize, cancellationToken);
    }
}
