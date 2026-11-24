using Mapster;
using MediatR;
using Sofra.Application.Common.Interfaces;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Menus.Dtos;
using Sofra.Domain.Entities;
using Sofra.Domain.Exceptions;

namespace Sofra.Application.Features.Menus.UpdateMenuItem;

public class UpdateMenuItemCommandHandler(ICurrentUser currentUser, IRestaurantOwnershipGuard ownershipGuard, IRepository<FoodItem> menuItems, IUnitOfWork unitOfWork) : IRequestHandler<UpdateMenuItemCommand, Result<MenuItemResponse>>
{
    public async Task<Result<MenuItemResponse>> Handle(UpdateMenuItemCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.GetUserId();
        var restaurantResult = await ownershipGuard.GetOwnedRestaurantAsync(userId, cancellationToken);
        if (restaurantResult.IsFailure) return Result<MenuItemResponse>.Failure(restaurantResult.Error!);
        var restaurant = restaurantResult.Response!;

        var item = await menuItems.FirstOrDefaultAsync(i => i.Id == request.FoodItemId && i.FoodType.RestaurantId == restaurant.Id, cancellationToken);
        if (item is null) return Result<MenuItemResponse>.Failure(Error.NotFound("MENU_ITEM_NOT_FOUND", "The food item was not found on your menu."));

        try { item.UpdateInfo(request.Name, request.Description, request.Price); }
        catch (DomainException ex) { return Result<MenuItemResponse>.Failure(Error.BusinessRule("MENU_ITEM_INFO_INVALID", ex.Message)); }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return item.Adapt<MenuItemResponse>();
    }
}
