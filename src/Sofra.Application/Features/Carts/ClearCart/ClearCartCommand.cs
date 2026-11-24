using MediatR;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Carts.Dtos;

namespace Sofra.Application.Features.Carts.ClearCart;

/// <summary>Empties the signed-in customer's cart completely (FR-22: used when switching restaurants).</summary>
public record ClearCartCommand : IRequest<Result<CartResponse>>;
