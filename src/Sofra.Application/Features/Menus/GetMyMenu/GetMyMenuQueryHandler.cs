using MediatR;
using Sofra.Application.Common.Interfaces;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Menus.Dtos;
using Sofra.Domain.Entities;

namespace Sofra.Application.Features.Menus.GetMyMenu;

public class GetMyMenuQueryHandler(
    ICurrentUser currentUser,
    IRestaurantOwnershipGuard ownershipGuard,
    IMenuRepository menus) : IRequestHandler<GetMyMenuQuery, Result<MyMenuResponse>>
{
    public async Task<Result<MyMenuResponse>> Handle(GetMyMenuQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.GetUserId();

        var restaurantResult = await ownershipGuard.GetOwnedRestaurantAsync(userId, cancellationToken);
        if (restaurantResult.IsFailure)
            return Result<MyMenuResponse>.Failure(restaurantResult.Error!);

        var restaurant = restaurantResult.Response!;

        return Result<MyMenuResponse>.Success(await menus.GetMyMenuAsync(restaurant.Id, cancellationToken));
    }
}
