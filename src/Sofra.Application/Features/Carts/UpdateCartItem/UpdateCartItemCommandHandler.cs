using MediatR;
using Sofra.Application.Common.Interfaces;
using Sofra.Application.Common.Results;
using Sofra.Domain.Entities;
using Sofra.Domain.Exceptions;
using Sofra.Application.Features.Carts.Dtos;

namespace Sofra.Application.Features.Carts.UpdateCartItem;

public class UpdateCartItemCommandHandler(ICurrentUser currentUser, ICartRepository carts, IUnitOfWork unitOfWork) : IRequestHandler<UpdateCartItemCommand, Result<CartResponse>>
{
    public async Task<Result<CartResponse>> Handle(UpdateCartItemCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.GetUserId();
        var cart = await carts.FindForUserAsync(userId, cancellationToken);
        if (cart is null) return Result<CartResponse>.Failure(Error.NotFound("CART_NOT_FOUND", "You do not have a cart yet."));

        try { cart.UpdateItemQuantity(request.FoodItemId, request.Quantity); }
        catch (DomainException ex) { return Result<CartResponse>.Failure(Error.NotFound("ITEM_NOT_IN_CART", ex.Message)); }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<CartResponse>.Success(await carts.GetForUserAsync(userId, cancellationToken) ?? new CartResponse(null, null, [], 0m));
    }
}
