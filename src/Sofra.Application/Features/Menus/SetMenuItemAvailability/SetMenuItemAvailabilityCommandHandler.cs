using Mapster;
using MediatR;
using Sofra.Application.Common.Interfaces;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Menus.Dtos;
using Sofra.Domain.Entities;

namespace Sofra.Application.Features.Menus.SetMenuItemAvailability;

public class SetMenuItemAvailabilityCommandHandler(ICurrentUser currentUser, IRestaurantOwnershipGuard ownershipGuard, IRepository<FoodItem> menuItems, IUnitOfWork unitOfWork) : IRequestHandler<SetMenuItemAvailabilityCommand, Result<MenuItemResponse>>
{
    public async Task<Result<MenuItemResponse>> Handle(SetMenuItemAvailabilityCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.GetUserId();
        var restaurantResult = await ownershipGuard.GetOwnedRestaurantAsync(userId, cancellationToken);
        if (restaurantResult.IsFailure) return Result<MenuItemResponse>.Failure(restaurantResult.Error!);
        var restaurant = restaurantResult.Response!;

        var item = await menuItems.FirstOrDefaultAsync(i => i.Id == request.FoodItemId && i.FoodType.RestaurantId == restaurant.Id, cancellationToken);
        if (item is null) return Result<MenuItemResponse>.Failure(Error.NotFound("MENU_ITEM_NOT_FOUND", "The food item was not found on your menu."));

        if (request.Available) item.MakeAvailable(); else item.MakeUnavailable();
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return item.Adapt<MenuItemResponse>();
    }
}
