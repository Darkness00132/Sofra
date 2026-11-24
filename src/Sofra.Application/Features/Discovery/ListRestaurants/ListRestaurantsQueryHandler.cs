using MediatR;
using Sofra.Application.Common.Interfaces;
using Sofra.Application.Common.Pagination;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Discovery.Dtos;

namespace Sofra.Application.Features.Discovery.ListRestaurants;

public class ListRestaurantsQueryHandler(IRestaurantRepository restaurants)
    : IRequestHandler<ListRestaurantsQuery, Result<PagedResult<PublicRestaurantResponse>>>
{
    public async Task<Result<PagedResult<PublicRestaurantResponse>>> Handle(ListRestaurantsQuery request, CancellationToken cancellationToken)
    {
        return await restaurants.ListPublicAsync(request.Filter, cancellationToken);
    }
}
