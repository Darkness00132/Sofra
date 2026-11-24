using MediatR;
using Sofra.Application.Common.Interfaces;
using Sofra.Application.Common.Pagination;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Orders.Dtos;

namespace Sofra.Application.Features.Orders.ListMyOrders;

public class ListMyOrdersQueryHandler(ICurrentUser currentUser, IOrderRepository orders)
    : IRequestHandler<ListMyOrdersQuery, Result<PagedResult<OrderSummaryResponse>>>
{
    public async Task<Result<PagedResult<OrderSummaryResponse>>> Handle(ListMyOrdersQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.GetUserId();

        return await orders.ListForCustomerAsync(userId, request.Filter, cancellationToken);
    }
}
