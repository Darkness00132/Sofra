using Mapster;
using MediatR;
using Sofra.Application.Common.Interfaces;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Menus.Dtos;
using Sofra.Domain.Entities;

namespace Sofra.Application.Features.Menus.UploadMenuItemImage;

public class UploadMenuItemImageCommandHandler(ICurrentUser currentUser, IRestaurantOwnershipGuard ownershipGuard, IRepository<FoodItem> menuItems, IFileStorage fileStorage, IUnitOfWork unitOfWork) : IRequestHandler<UploadMenuItemImageCommand, Result<MenuItemResponse>>
{
    public async Task<Result<MenuItemResponse>> Handle(UploadMenuItemImageCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.GetUserId();
        var restaurantResult = await ownershipGuard.GetOwnedRestaurantAsync(userId, cancellationToken);
        if (restaurantResult.IsFailure) return Result<MenuItemResponse>.Failure(restaurantResult.Error!);
        var restaurant = restaurantResult.Response!;

        var item = await menuItems.FirstOrDefaultAsync(i => i.Id == request.FoodItemId && i.FoodType.RestaurantId == restaurant.Id, cancellationToken);
        if (item is null) return Result<MenuItemResponse>.Failure(Error.NotFound("MENU_ITEM_NOT_FOUND", "The food item was not found on your menu."));

        var previousImagePublicId = item.ImagePublicId;
        var stored = await fileStorage.UploadAsync(request.Image, cancellationToken);
        item.SetImage(stored.Url, stored.StorageId);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        if (previousImagePublicId is not null) await fileStorage.DeleteAsync(previousImagePublicId, cancellationToken);
        return item.Adapt<MenuItemResponse>();
    }
}
