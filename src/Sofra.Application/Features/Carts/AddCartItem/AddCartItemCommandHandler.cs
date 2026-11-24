using MediatR;
using Sofra.Application.Common.Interfaces;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Carts.Dtos;
using Sofra.Domain.Common;
using Sofra.Domain.Entities;
using Sofra.Domain.Exceptions;

namespace Sofra.Application.Features.Carts.AddCartItem;

public class AddCartItemCommandHandler(ICurrentUser currentUser, IFoodItemRepository foodItems, ICartRepository carts, IUnitOfWork unitOfWork) : IRequestHandler<AddCartItemCommand, Result<CartResponse>>
{
    public async Task<Result<CartResponse>> Handle(AddCartItemCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.GetUserId();
        var foodItem = await foodItems.FindForCartAsync(request.FoodItemId, cancellationToken);
        if (foodItem is null) return Result<CartResponse>.Failure(Error.NotFound("FOOD_ITEM_NOT_FOUND", "The food item was not found."));
        if (foodItem.RestaurantId == Guid.Empty) return Result<CartResponse>.Failure(Error.NotFound("FOOD_TYPE_NOT_FOUND", "This food item's menu category was not found."));
        if (!foodItem.IsAvailable) return Result<CartResponse>.Failure(Error.BusinessRule("FOOD_ITEM_UNAVAILABLE", $"'{foodItem.Name}' is no longer available (BR-05); remove it and try again."));

        var cart = await carts.FindForUserAsync(userId, cancellationToken);
        if (cart is null) { cart = Cart.Create(userId, foodItem.RestaurantId); await carts.AddAsync(cart, cancellationToken); }

        try { cart.AddItem(new CartItemSource(foodItem.Id, foodItem.RestaurantId, foodItem.Name, foodItem.IsAvailable), request.Quantity); }
        catch (DomainException ex) { return Result<CartResponse>.Failure(Error.Conflict("CART_RESTAURANT_MISMATCH", ex.Message)); }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<CartResponse>.Success(await carts.GetForUserAsync(userId, cancellationToken) ?? new CartResponse(null, null, [], 0m));
    }
} 