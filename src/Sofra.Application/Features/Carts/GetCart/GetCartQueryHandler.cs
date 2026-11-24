using MediatR;
using Sofra.Application.Common.Interfaces;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Carts.Dtos;

namespace Sofra.Application.Features.Carts.GetCart;

public class GetCartQueryHandler(ICurrentUser currentUser, ICartRepository carts) : IRequestHandler<GetCartQuery, Result<CartResponse>>
{
    public async Task<Result<CartResponse>> Handle(GetCartQuery request, CancellationToken cancellationToken) =>
        Result<CartResponse>.Success(await carts.GetForUserAsync(currentUser.GetUserId(), cancellationToken) ?? new CartResponse(null, null, [], 0m));
}
