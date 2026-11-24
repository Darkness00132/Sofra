using Mapster;
using MediatR;
using Sofra.Application.Common.Interfaces;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Menus.Dtos;
using Sofra.Domain.Entities;

namespace Sofra.Application.Features.Menus.AddMenuItem;

public class AddMenuItemCommandHandler(
    ICurrentUser currentUser,
    IRestaurantOwnershipGuard ownershipGuard,
    IRepository<FoodType> menuTypes,
    IRepository<FoodItem> menuItems,
    IUnitOfWork unitOfWork) : IRequestHandler<AddMenuItemCommand, Result<MenuItemResponse>>
{
    public async Task<Result<MenuItemResponse>> Handle(AddMenuItemCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.GetUserId();

        var restaurantResult = await ownershipGuard.GetOwnedRestaurantAsync(userId, cancellationToken);
        if (restaurantResult.IsFailure)
            return Result<MenuItemResponse>.Failure(restaurantResult.Error!);

        var restaurant = restaurantResult.Response!;

        var menuType = await menuTypes.FirstOrDefaultAsync(
            t => t.Id == request.MenuTypeId && t.RestaurantId == restaurant.Id,
            cancellationToken);
        if (menuType is null)
            return Result<MenuItemResponse>.Failure(Error.NotFound(
                "MENU_TYPE_NOT_FOUND",
                "The menu type was not found on your menu."));

        var item = FoodItem.Create(request.MenuTypeId, request.Name, request.Description, request.Price, request.Availability);

        await menuItems.AddAsync(item, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return item.Adapt<MenuItemResponse>();
    }
}
