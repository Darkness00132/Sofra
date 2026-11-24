using Mapster;
using MediatR;
using Sofra.Application.Common.Interfaces;
using Sofra.Application.Common.Results;
using Sofra.Domain.Entities;
using Sofra.Domain.Enums;
using Sofra.Domain.Exceptions;
using Sofra.Application.Features.Orders.Dtos;

namespace Sofra.Application.Features.Orders.CancelOrder;

public class CancelOrderCommandHandler(
    ICurrentUser currentUser,
    IOrderRepository orders,
    IUnitOfWork unitOfWork) : IRequestHandler<CancelOrderCommand, Result<OrderDetailsResponse>>
{
    public async Task<Result<OrderDetailsResponse>> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.GetUserId();

        var order = await orders.FindWithDetailsAsync(request.OrderId, cancellationToken);
        if (order is null || order.CustomerId != userId)
            return Result<OrderDetailsResponse>.Failure(Error.NotFound(
                "ORDER_NOT_FOUND",
                "The order was not found."));

        // FR-39: pending orders can always be cancelled. Cancellation after acceptance is an
        // open product decision (PRD §10) and stays unavailable until it is made.
        if (order.Status != OrderStatus.Pending)
            return Result<OrderDetailsResponse>.Failure(Error.BusinessRule(
                "CANCELLATION_NOT_ALLOWED",
                "Only orders that are still pending can be cancelled; contact the restaurant instead."));

        try
        {
            order.Cancel(request.Reason, request.Note, userId);
        }
        catch (DomainException ex)
        {
            return Result<OrderDetailsResponse>.Failure(Error.BusinessRule("CANCELLATION_NOT_ALLOWED", ex.Message));
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return order.Adapt<OrderDetailsResponse>();
    }
}
