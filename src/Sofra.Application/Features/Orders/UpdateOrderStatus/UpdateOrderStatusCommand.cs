using MediatR;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Orders.Dtos;
using Sofra.Domain.Enums;

namespace Sofra.Application.Features.Orders.UpdateOrderStatus;

/// <summary>
/// Moves one of the signed-in owner's restaurant orders to its next status (FR-33…FR-35,
/// BR-06, BR-08): Accepted, Rejected (reason required), Preparing, Ready or Completed.
/// Cash payments are marked completed when the order completes.
/// </summary>
public record UpdateOrderStatusCommand(
    Guid OrderId,
    OrderStatus Status,
    string? Reason) : IRequest<Result<OrderDetailsResponse>>;
