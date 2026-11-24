using MediatR;
using Sofra.Application.Common.Interfaces;
using Sofra.Application.Common.Results;
using Sofra.Domain.Entities;
using Sofra.Domain.Exceptions;
using Sofra.Application.Features.Carts.Dtos;

namespace Sofra.Application.Features.Carts.RemoveCartItem;

public class RemoveCartItemCommandHandler(ICurrentUser currentUser, ICartRepository carts, IUnitOfWork unitOfWork) : IRequestHandler<RemoveCartItemCommand, Result<CartResponse>>
{
    public async Task<Result<CartResponse>> Handle(RemoveCartItemCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.GetUserId();
        var cart = await carts.FindForUserAsync(userId, cancellationToken);
        if (cart is null) return Result<CartResponse>.Failure(Error.NotFound("CART_NOT_FOUND", "You do not have a cart yet."));

        try { cart.RemoveItem(request.FoodItemId); }
        catch (DomainException ex) { return Result<CartResponse>.Failure(Error.NotFound("ITEM_NOT_IN_CART", ex.Message)); }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<CartResponse>.Success(await carts.GetForUserAsync(userId, cancellationToken) ?? new CartResponse(null, null, [], 0m));
    }
}
