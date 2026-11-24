using Mapster;
using MediatR;
using Sofra.Application.Common.Interfaces;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Restaurants.Dtos;
using Sofra.Domain.Enums;
using Sofra.Domain.Exceptions;

namespace Sofra.Application.Features.Restaurants.Admin.UpdateRestaurantStatus;

public class UpdateRestaurantStatusCommandHandler(IRestaurantRepository restaurants, IUnitOfWork unitOfWork) : IRequestHandler<UpdateRestaurantStatusCommand, Result<RestaurantResponse>>
{
    public async Task<Result<RestaurantResponse>> Handle(UpdateRestaurantStatusCommand request, CancellationToken cancellationToken)
    {
        var restaurant = await restaurants.FindWithOwnerAsync(request.RestaurantId, cancellationToken);
        if (restaurant is null) return Result<RestaurantResponse>.Failure(Error.NotFound("RESTAURANT_NOT_FOUND", "The restaurant was not found."));

        try
        {
            switch (request.Status)
            {
                case RestaurantStatus.Suspended: restaurant.Suspend(); break;
                case RestaurantStatus.Approved: restaurant.Reinstate(); break;
                default: return Result<RestaurantResponse>.Failure(Error.Validation("STATUS_NOT_SUPPORTED", "Only Suspended or Approved are supported for restaurants."));
            }
        }
        catch (DomainException ex) { return Result<RestaurantResponse>.Failure(Error.BusinessRule("RESTAURANT_STATUS_INVALID", ex.Message)); }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return restaurant.Adapt<RestaurantResponse>();
    }
}
