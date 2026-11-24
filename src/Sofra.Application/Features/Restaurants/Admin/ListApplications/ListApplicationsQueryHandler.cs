using MediatR;
using Sofra.Application.Common.Interfaces;
using Sofra.Application.Common.Pagination;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Restaurants.Dtos;

namespace Sofra.Application.Features.Restaurants.Admin.ListApplications;

public class ListApplicationsQueryHandler(IRestaurantApplicationRepository applications)
    : IRequestHandler<ListApplicationsQuery, Result<PagedResult<ApplicationResponse>>>
{
    public async Task<Result<PagedResult<ApplicationResponse>>> Handle(ListApplicationsQuery request, CancellationToken cancellationToken)
    {
        return await applications.ListAsync(request.Filter, cancellationToken);
    }
}
