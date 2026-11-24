using MediatR;
using Sofra.Application.Common.Interfaces;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Carts.Dtos;

namespace Sofra.Application.Features.Carts.ClearCart;

public class ClearCartCommandHandler(ICurrentUser currentUser, ICartRepository carts, IUnitOfWork unitOfWork) : IRequestHandler<ClearCartCommand, Result<CartResponse>>
{
    public async Task<Result<CartResponse>> Handle(ClearCartCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.GetUserId();
        var cart = await carts.FindForUserAsync(userId, cancellationToken);
        if (cart is not null) { cart.Clear(); await unitOfWork.SaveChangesAsync(cancellationToken); }
        return Result<CartResponse>.Success(await carts.GetForUserAsync(userId, cancellationToken) ?? new CartResponse(null, null, [], 0m));
    }
}
