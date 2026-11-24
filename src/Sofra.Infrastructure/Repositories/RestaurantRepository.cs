using Mapster;
using Microsoft.EntityFrameworkCore;
using Sofra.Application.Common.Interfaces;
using Sofra.Application.Common.Pagination;
using Sofra.Application.Features.Discovery;
using Sofra.Application.Features.Discovery.Dtos;
using Sofra.Application.Features.Restaurants.Admin.ListRestaurants;
using Sofra.Application.Features.Restaurants.Dtos;
using Sofra.Domain.Entities;
using Sofra.Domain.Enums;
using Sofra.Infrastructure.Repositories.Extensions;
using Sofra.Infrastructure.Persistent;

namespace Sofra.Infrastructure.Repositories;

public class RestaurantRepository(SofraDbContext context)
    : Repository<Restaurant>(context), IRestaurantRepository
{
    public async Task<PagedResult<RestaurantResponse>> ListAsync(RestaurantListFilter filter, CancellationToken cancellationToken = default)
    {
        var query = Context.Restaurants
            .ApplyFilter(filter)
            .ProjectToType<RestaurantResponse>(TypeAdapterConfig.GlobalSettings);

        return await this.ToPagedResultAsync(query, filter.PageNumber, filter.PageSize, cancellationToken);
    }

    public async Task<PagedResult<PublicRestaurantResponse>> ListPublicAsync(PublicRestaurantFilter filter, CancellationToken cancellationToken = default)
    {
        var query = Context.Restaurants
            .Where(r => r.Status == RestaurantStatus.Approved)
            .ApplyFilter(filter)
            .ProjectToType<PublicRestaurantResponse>(TypeAdapterConfig.GlobalSettings);

        return await this.ToPagedResultAsync(query, filter.PageNumber, filter.PageSize, cancellationToken);
    }

    public Task<Restaurant?> FindWithOwnerAsync(Guid id, CancellationToken cancellationToken = default) =>
        Context.Restaurants
            .Include(r => r.Owner)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

    public async Task<RestaurantStatusCounts> GetStatusCountsAsync(CancellationToken cancellationToken = default)
    {
        var counts = await Context.Restaurants
            .GroupBy(r => r.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        return new RestaurantStatusCounts(
            counts.Sum(c => c.Count),
            counts.FirstOrDefault(c => c.Status == RestaurantStatus.Approved)?.Count ?? 0,
            counts.FirstOrDefault(c => c.Status == RestaurantStatus.Suspended)?.Count ?? 0);
    }
}
