using MediatR;
using Sofra.Application.Common.Results;
using Sofra.Domain.Enums;
using Sofra.Application.Features.Orders.Dtos;

namespace Sofra.Application.Features.Orders.CancelOrder;

/// <summary>Cancels the signed-in customer's order while the cancellation policy allows it (FR-39…FR-41, BR-07).</summary>
public record CancelOrderCommand(Guid OrderId, OrderCancellationReason Reason, string? Note) : IRequest<Result<OrderDetailsResponse>>;
