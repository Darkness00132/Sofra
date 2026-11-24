using MediatR;
using Sofra.Application.Common.Interfaces;
using Sofra.Application.Common.Pagination;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Restaurants.Dtos;

namespace Sofra.Application.Features.Restaurants.Admin.ListRestaurants;

public class ListRestaurantsQueryHandler(IRestaurantRepository restaurants)
    : IRequestHandler<ListRestaurantsQuery, Result<PagedResult<RestaurantResponse>>>
{
    public async Task<Result<PagedResult<RestaurantResponse>>> Handle(ListRestaurantsQuery request, CancellationToken cancellationToken)
    {
        return await restaurants.ListAsync(request.Filter, cancellationToken);
    }
}
