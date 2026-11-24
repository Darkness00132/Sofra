using Mapster;
using MediatR;
using Sofra.Application.Common.Interfaces;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Restaurants.Dtos;
using Sofra.Domain.Entities;
using Sofra.Domain.Exceptions;

namespace Sofra.Application.Features.Restaurants.UpdateMyRestaurant;

public class UpdateMyRestaurantCommandHandler(
    ICurrentUser currentUser,
    IRestaurantOwnershipGuard ownershipGuard,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateMyRestaurantCommand, Result<MyRestaurantResponse>>
{
    public async Task<Result<MyRestaurantResponse>> Handle(UpdateMyRestaurantCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.GetUserId();

        var restaurantResult = await ownershipGuard.GetOwnedRestaurantAsync(userId, cancellationToken);
        if (restaurantResult.IsFailure)
            return Result<MyRestaurantResponse>.Failure(restaurantResult.Error!);

        var restaurant = restaurantResult.Response!;

        try
        {
            restaurant.UpdateInfo(request.Name, request.Address, request.Phone, request.Description);
        }
        catch (DomainException ex)
        {
            return Result<MyRestaurantResponse>.Failure(Error.BusinessRule("RESTAURANT_INFO_INVALID", ex.Message));
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return restaurant.Adapt<MyRestaurantResponse>();
    }
}
