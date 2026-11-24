using Mapster;
using MediatR;
using Sofra.Application.Common.Interfaces;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Menus.Dtos;
using Sofra.Domain.Entities;

namespace Sofra.Application.Features.Menus.AddMenuType;

public class AddMenuTypeCommandHandler(
    ICurrentUser currentUser,
    IRestaurantOwnershipGuard ownershipGuard,
    IRepository<FoodCategory> categories,
    IRepository<FoodType> menuTypes,
    IUnitOfWork unitOfWork) : IRequestHandler<AddMenuTypeCommand, Result<MenuTypeResponse>>
{
    public async Task<Result<MenuTypeResponse>> Handle(AddMenuTypeCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.GetUserId();

        var restaurantResult = await ownershipGuard.GetOwnedRestaurantAsync(userId, cancellationToken);
        if (restaurantResult.IsFailure)
            return Result<MenuTypeResponse>.Failure(restaurantResult.Error!);

        var restaurant = restaurantResult.Response!;

        if (await categories.FindAsync(request.FoodCategoryId, cancellationToken) is null)
            return Result<MenuTypeResponse>.Failure(Error.NotFound(
                "FOOD_CATEGORY_NOT_FOUND",
                "The food category was not found."));

        var menuType = FoodType.Create(restaurant.Id, request.FoodCategoryId, request.Name, request.Description);

        await menuTypes.AddAsync(menuType, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return menuType.Adapt<MenuTypeResponse>();
    }
}
