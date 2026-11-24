using Mapster;
using Microsoft.EntityFrameworkCore;
using Sofra.Application.Common.Interfaces;
using Sofra.Application.Common.Pagination;
using Sofra.Application.Features.Restaurants.Admin.ListApplications;
using Sofra.Application.Features.Restaurants.Dtos;
using Sofra.Domain.Entities;
using Sofra.Domain.Enums;
using Sofra.Infrastructure.Repositories.Extensions;
using Sofra.Infrastructure.Persistent;

namespace Sofra.Infrastructure.Repositories;

public class RestaurantApplicationRepository(SofraDbContext context)
    : Repository<RestaurantApplication>(context), IRestaurantApplicationRepository
{
    public async Task<PagedResult<ApplicationResponse>> ListAsync(ApplicationListFilter filter, CancellationToken cancellationToken = default)
    {
        var query = Context.RestaurantApplications
            .ApplyFilter(filter)
            .ProjectToType<ApplicationResponse>(TypeAdapterConfig.GlobalSettings);

        return await this.ToPagedResultAsync(query, filter.PageNumber, filter.PageSize, cancellationToken);
    }

    public Task<RestaurantApplication?> FindWithApplicantAsync(Guid id, CancellationToken cancellationToken = default) =>
        Context.RestaurantApplications
            .Include(a => a.Applicant)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public Task<int> CountPendingAsync(CancellationToken cancellationToken = default) =>
        Context.RestaurantApplications.CountAsync(a => a.Status == RestaurantStatus.PendingApproval, cancellationToken);
}
