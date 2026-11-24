using MediatR;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Carts.Dtos;

namespace Sofra.Application.Features.Carts.GetCart;

/// <summary>Returns the signed-in customer's cart with prices and subtotal (FR-23).</summary>
public record GetCartQuery : IRequest<Result<CartResponse>>;
