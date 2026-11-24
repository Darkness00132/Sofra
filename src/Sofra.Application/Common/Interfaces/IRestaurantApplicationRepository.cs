using Sofra.Application.Common.Pagination;
using Sofra.Application.Features.Restaurants.Admin.ListApplications;
using Sofra.Application.Features.Restaurants.Dtos;
using Sofra.Domain.Entities;

namespace Sofra.Application.Common.Interfaces;

/// <summary>
/// Restaurant-application repository — only the paged, filtered admin list and the pending
/// count for the dashboard go beyond the generic repository's common operations.
/// </summary>
public interface IRestaurantApplicationRepository : IRepository<RestaurantApplication>
{
    /// <summary>Admin list: applies the filter, ordered newest first, applicant email joined from the database.</summary>
    Task<PagedResult<ApplicationResponse>> ListAsync(ApplicationListFilter filter, CancellationToken cancellationToken = default);

    /// <summary>Finds one application with its applicant loaded, so the response has the email without a second query.</summary>
    Task<RestaurantApplication?> FindWithApplicantAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Counts applications waiting for review (dashboard).</summary>
    Task<int> CountPendingAsync(CancellationToken cancellationToken = default);
}
