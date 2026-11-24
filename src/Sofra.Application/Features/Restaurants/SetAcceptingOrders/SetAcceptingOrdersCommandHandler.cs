using Mapster;
using MediatR;
using Sofra.Application.Common.Interfaces;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Restaurants.Dtos;
using Sofra.Domain.Entities;

namespace Sofra.Application.Features.Restaurants.SetAcceptingOrders;

public class SetAcceptingOrdersCommandHandler(
    ICurrentUser currentUser,
    IRestaurantOwnershipGuard ownershipGuard,
    IUnitOfWork unitOfWork) : IRequestHandler<SetAcceptingOrdersCommand, Result<MyRestaurantResponse>>
{
    public async Task<Result<MyRestaurantResponse>> Handle(SetAcceptingOrdersCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.GetUserId();

        var restaurantResult = await ownershipGuard.GetOwnedRestaurantAsync(userId, cancellationToken);
        if (restaurantResult.IsFailure)
            return Result<MyRestaurantResponse>.Failure(restaurantResult.Error!);

        var restaurant = restaurantResult.Response!;

        restaurant.SetAcceptingOrders(request.AcceptingOrders);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return restaurant.Adapt<MyRestaurantResponse>();
    }
}
