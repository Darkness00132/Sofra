using Mapster;
using MediatR;
using Sofra.Application.Common.Interfaces;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Menus.Dtos;
using Sofra.Domain.Entities;
using Sofra.Domain.Exceptions;

namespace Sofra.Application.Features.Menus.UpdateMenuType;

public class UpdateMenuTypeCommandHandler(
    ICurrentUser currentUser,
    IRestaurantOwnershipGuard ownershipGuard,
    IRepository<FoodType> menuTypes,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateMenuTypeCommand, Result<MenuTypeResponse>>
{
    public async Task<Result<MenuTypeResponse>> Handle(UpdateMenuTypeCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.GetUserId();

        var restaurantResult = await ownershipGuard.GetOwnedRestaurantAsync(userId, cancellationToken);
        if (restaurantResult.IsFailure)
            return Result<MenuTypeResponse>.Failure(restaurantResult.Error!);

        var restaurant = restaurantResult.Response!;

        var menuType = await menuTypes.FirstOrDefaultAsync(
            t => t.Id == request.MenuTypeId && t.RestaurantId == restaurant.Id,
            cancellationToken);
        if (menuType is null)
            return Result<MenuTypeResponse>.Failure(Error.NotFound(
                "MENU_TYPE_NOT_FOUND",
                "The menu type was not found on your menu."));

        try
        {
            menuType.UpdateInfo(request.Name, request.Description);
        }
        catch (DomainException ex)
        {
            return Result<MenuTypeResponse>.Failure(Error.BusinessRule("MENU_TYPE_INFO_INVALID", ex.Message));
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return menuType.Adapt<MenuTypeResponse>();
    }
}
