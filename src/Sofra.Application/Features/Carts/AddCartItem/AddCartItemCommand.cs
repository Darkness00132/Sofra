using MediatR;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Carts.Dtos;

namespace Sofra.Application.Features.Carts.AddCartItem;

/// <summary>Adds a food item to the signed-in customer's cart (FR-19). Cross-restaurant adds are rejected (FR-22, BR-02).</summary>
public record AddCartItemCommand(Guid FoodItemId, int Quantity) : IRequest<Result<CartResponse>>;
