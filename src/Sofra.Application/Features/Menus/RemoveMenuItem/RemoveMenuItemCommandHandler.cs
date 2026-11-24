using MediatR;
using Sofra.Application.Common.Interfaces;
using Sofra.Application.Common.Results;
using Sofra.Domain.Entities;

namespace Sofra.Application.Features.Menus.RemoveMenuItem;

public class RemoveMenuItemCommandHandler(ICurrentUser currentUser, IRestaurantOwnershipGuard ownershipGuard, IRepository<FoodItem> menuItems, IOrderRepository orders, IUnitOfWork unitOfWork) : IRequestHandler<RemoveMenuItemCommand, Result>
{
    public async Task<Result> Handle(RemoveMenuItemCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.GetUserId();
        var restaurantResult = await ownershipGuard.GetOwnedRestaurantAsync(userId, cancellationToken);
        if (restaurantResult.IsFailure) return Result.Failure(restaurantResult.Error!);
        var restaurant = restaurantResult.Response!;

        var item = await menuItems.FirstOrDefaultAsync(i => i.Id == request.FoodItemId && i.FoodType.RestaurantId == restaurant.Id, cancellationToken);
        if (item is null) return Result.Failure(Error.NotFound("MENU_ITEM_NOT_FOUND", "The food item was not found on your menu."));
        if (await orders.HasOrdersForFoodItemAsync(item.Id, cancellationToken)) return Result.Failure(Error.Conflict("MENU_ITEM_HAS_ORDER_HISTORY", "This item is part of past orders and cannot be removed; mark it unavailable instead."));

        menuItems.Delete(item);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
