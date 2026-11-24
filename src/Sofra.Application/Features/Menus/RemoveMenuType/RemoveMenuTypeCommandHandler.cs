using MediatR;
using Sofra.Application.Common.Interfaces;
using Sofra.Application.Common.Results;
using Sofra.Domain.Entities;

namespace Sofra.Application.Features.Menus.RemoveMenuType;

public class RemoveMenuTypeCommandHandler(
    ICurrentUser currentUser,
    IRestaurantOwnershipGuard ownershipGuard,
    IRepository<FoodType> menuTypes,
    IRepository<FoodItem> menuItems,
    IUnitOfWork unitOfWork) : IRequestHandler<RemoveMenuTypeCommand, Result>
{
    public async Task<Result> Handle(RemoveMenuTypeCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.GetUserId();

        var restaurantResult = await ownershipGuard.GetOwnedRestaurantAsync(userId, cancellationToken);
        if (restaurantResult.IsFailure)
            return Result.Failure(restaurantResult.Error!);

        var restaurant = restaurantResult.Response!;

        var menuType = await menuTypes.FirstOrDefaultAsync(
            t => t.Id == request.MenuTypeId && t.RestaurantId == restaurant.Id,
            cancellationToken);
        if (menuType is null)
            return Result.Failure(Error.NotFound(
                "MENU_TYPE_NOT_FOUND",
                "The menu type was not found on your menu."));

        if (await menuItems.ExistsAsync(i => i.FoodTypeId == request.MenuTypeId, cancellationToken))
            return Result.Failure(Error.Conflict(
                "MENU_TYPE_HAS_ITEMS",
                "Remove the food items on this menu type first."));

        menuTypes.Delete(menuType);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
