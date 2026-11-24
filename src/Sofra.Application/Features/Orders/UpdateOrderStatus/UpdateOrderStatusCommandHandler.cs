using Mapster;
using MediatR;
using Sofra.Application.Common.Interfaces;
using Sofra.Application.Common.Results;
using Sofra.Domain.Entities;
using Sofra.Domain.Enums;
using Sofra.Domain.Exceptions;
using Sofra.Application.Features.Orders.Dtos;

namespace Sofra.Application.Features.Orders.UpdateOrderStatus;

public class UpdateOrderStatusCommandHandler(
    ICurrentUser currentUser,
    IRestaurantOwnershipGuard ownershipGuard,
    IOrderRepository orders,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateOrderStatusCommand, Result<OrderDetailsResponse>>
{
    public async Task<Result<OrderDetailsResponse>> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.GetUserId();

        var restaurantResult = await ownershipGuard.GetOwnedRestaurantAsync(userId, cancellationToken);
        if (restaurantResult.IsFailure)
            return Result<OrderDetailsResponse>.Failure(restaurantResult.Error!);

        var restaurant = restaurantResult.Response!;

        var order = await orders.FindWithDetailsAsync(request.OrderId, cancellationToken);
        if (order is null || order.RestaurantId != restaurant.Id)
            return Result<OrderDetailsResponse>.Failure(Error.NotFound(
                "ORDER_NOT_FOUND",
                "The order was not found."));

        try
        {
            switch (request.Status)
            {
                case OrderStatus.Accepted:
                    order.Accept(userId);
                    break;
                case OrderStatus.Rejected:
                    order.Reject(request.Reason!, userId);
                    break;
                case OrderStatus.Preparing:
                    order.StartPreparing(userId);
                    break;
                case OrderStatus.Ready:
                    order.MarkReady(userId);
                    break;
                case OrderStatus.Completed:
                    order.Complete(userId);
                    CompleteCashPayment(order);
                    break;
                default:
                    return Result<OrderDetailsResponse>.Failure(Error.Validation(
                        "STATUS_NOT_SUPPORTED",
                        "Status must be Accepted, Rejected, Preparing, Ready or Completed."));
            }
        }
        catch (DomainException ex)
        {
            return Result<OrderDetailsResponse>.Failure(Error.BusinessRule("ORDER_STATUS_INVALID", ex.Message));
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return order.Adapt<OrderDetailsResponse>();
    }

    /// <summary>Cash payments are collected when the order completes (FR-27, BR-11).</summary>
    private static void CompleteCashPayment(Order order)
    {
        var cashPayment = order.Payments.FirstOrDefault(p =>
            p.PaymentMethod == PaymentMethod.CashOnDelivery && p.Status == PaymentStatus.Pending);

        cashPayment?.MarkCompleted();
    }
}
