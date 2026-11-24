using MediatR;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Carts.Dtos;

namespace Sofra.Application.Features.Carts.UpdateCartItem;

/// <summary>Changes the quantity of one item in the signed-in customer's cart (FR-20).</summary>
public record UpdateCartItemCommand(Guid FoodItemId, int Quantity) : IRequest<Result<CartResponse>>;
