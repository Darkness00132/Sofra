using MediatR;
using Sofra.Application.Common.Interfaces;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Orders.Dtos;

namespace Sofra.Application.Features.Orders.GetOrderDetails;

public class GetOrderDetailsQueryHandler(
    ICurrentUser currentUser,
    IOrderRepository orders) : IRequestHandler<GetOrderDetailsQuery, Result<OrderDetailsResponse>>
{
    public async Task<Result<OrderDetailsResponse>> Handle(GetOrderDetailsQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.GetUserId();

        var order = await orders.GetDetailsAsync(request.OrderId, cancellationToken);
        if (order is null || order.CustomerId != userId)
            return Result<OrderDetailsResponse>.Failure(Error.NotFound(
                "ORDER_NOT_FOUND",
                "The order was not found."));

        return order;
    }
}
