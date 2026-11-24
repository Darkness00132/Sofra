using Mapster;
using MediatR;
using Sofra.Application.Common.Interfaces;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Menus.Dtos;
using Sofra.Domain.Entities;

namespace Sofra.Application.Features.Menus.RemoveMenuItemImage;

public class RemoveMenuItemImageCommandHandler(ICurrentUser currentUser, IRestaurantOwnershipGuard ownershipGuard, IRepository<FoodItem> menuItems, IFileStorage fileStorage, IUnitOfWork unitOfWork) : IRequestHandler<RemoveMenuItemImageCommand, Result<MenuItemResponse>>
{
    public async Task<Result<MenuItemResponse>> Handle(RemoveMenuItemImageCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.GetUserId();
        var restaurantResult = await ownershipGuard.GetOwnedRestaurantAsync(userId, cancellationToken);
        if (restaurantResult.IsFailure) return Result<MenuItemResponse>.Failure(restaurantResult.Error!);
        var restaurant = restaurantResult.Response!;

        var item = await menuItems.FirstOrDefaultAsync(i => i.Id == request.FoodItemId && i.FoodType.RestaurantId == restaurant.Id, cancellationToken);
        if (item is null) return Result<MenuItemResponse>.Failure(Error.NotFound("MENU_ITEM_NOT_FOUND", "The food item was not found on your menu."));
        if (item.ImagePublicId is null) return Result<MenuItemResponse>.Failure(Error.BusinessRule("MENU_ITEM_IMAGE_NOT_FOUND", "The food item has no picture."));

        var removedImagePublicId = item.ImagePublicId;
        item.ClearImage();
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await fileStorage.DeleteAsync(removedImagePublicId, cancellationToken);
        return item.Adapt<MenuItemResponse>();
    }
}
