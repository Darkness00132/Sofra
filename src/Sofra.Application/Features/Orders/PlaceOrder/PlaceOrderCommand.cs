using MediatR;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Orders.Dtos;
using Sofra.Domain.Enums;

namespace Sofra.Application.Features.Orders.PlaceOrder;

/// <summary>
/// Checks out the signed-in customer's cart as an order (FR-25…FR-31): re-validates
/// availability, snapshots prices, applies fees, records the payment, clears the cart.
/// </summary>
public record PlaceOrderCommand(
    FulfilmentType FulfilmentType,
    PaymentMethod PaymentMethod,
    string? DeliveryAddress) : IRequest<Result<PlaceOrderResponse>>;
