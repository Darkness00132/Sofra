using MediatR;
using Sofra.Application.Common.Interfaces;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Orders.Dtos;

namespace Sofra.Application.Features.Admin.GetOrderDetails;

public class GetOrderDetailsQueryHandler(IOrderRepository orders)
    : IRequestHandler<GetOrderDetailsQuery, Result<OrderDetailsResponse>>
{
    public async Task<Result<OrderDetailsResponse>> Handle(GetOrderDetailsQuery request, CancellationToken cancellationToken)
    {
        var order = await orders.GetDetailsAsync(request.OrderId, cancellationToken);
        if (order is null)
            return Result<OrderDetailsResponse>.Failure(Error.NotFound(
                "ORDER_NOT_FOUND",
                "The order was not found."));

        return order;
    }
}
