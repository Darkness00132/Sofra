using Sofra.Application.Common.Pagination;
using Sofra.Application.Features.Discovery;
using Sofra.Application.Features.Discovery.Dtos;
using Sofra.Application.Features.Restaurants.Admin.ListRestaurants;
using Sofra.Application.Features.Restaurants.Dtos;
using Sofra.Domain.Entities;

namespace Sofra.Application.Common.Interfaces;

/// <summary>
/// Restaurant repository — only the paged, filtered admin/public lists and the dashboard
/// status counts go beyond the generic repository's common operations.
/// </summary>
public interface IRestaurantRepository : IRepository<Restaurant>
{
    /// <summary>Admin list: applies the filter, ordered by name, owner email joined from the database.</summary>
    Task<PagedResult<RestaurantResponse>> ListAsync(RestaurantListFilter filter, CancellationToken cancellationToken = default);

    /// <summary>Public discovery list: approved restaurants only, applies the filter, ordered by name.</summary>
    Task<PagedResult<PublicRestaurantResponse>> ListPublicAsync(PublicRestaurantFilter filter, CancellationToken cancellationToken = default);


    /// <summary>Finds one restaurant with its owner loaded, for responses showing the owner email.</summary>
    Task<Restaurant?> FindWithOwnerAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Status counts for the administrator dashboard.</summary>
    Task<RestaurantStatusCounts> GetStatusCountsAsync(CancellationToken cancellationToken = default);
}

/// <summary>How many restaurants sit in each of the statuses the admin dashboard shows.</summary>
public record RestaurantStatusCounts(int Total, int Approved, int Suspended);
