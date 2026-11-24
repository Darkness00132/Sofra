using MediatR;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Carts.Dtos;

namespace Sofra.Application.Features.Carts.RemoveCartItem;

/// <summary>Removes one item from the signed-in customer's cart (FR-20).</summary>
public record RemoveCartItemCommand(Guid FoodItemId) : IRequest<Result<CartResponse>>;
