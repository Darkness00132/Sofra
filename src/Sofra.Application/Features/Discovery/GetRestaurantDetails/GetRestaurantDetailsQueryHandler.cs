using Mapster;
using MediatR;
using Sofra.Application.Common.Interfaces;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Discovery.Dtos;
using Sofra.Domain.Enums;

namespace Sofra.Application.Features.Discovery.GetRestaurantDetails;

public class GetRestaurantDetailsQueryHandler(IRestaurantRepository restaurants)
    : IRequestHandler<GetRestaurantDetailsQuery, Result<PublicRestaurantResponse>>
{
    public async Task<Result<PublicRestaurantResponse>> Handle(GetRestaurantDetailsQuery request, CancellationToken cancellationToken)
    {
        var restaurant = await restaurants.FindAsync(request.RestaurantId, cancellationToken);
        if (restaurant is null || restaurant.Status != RestaurantStatus.Approved)
            return Result<PublicRestaurantResponse>.Failure(Error.NotFound(
                "RESTAURANT_NOT_FOUND",
                "The restaurant was not found."));

        return restaurant.Adapt<PublicRestaurantResponse>();
    }
}
