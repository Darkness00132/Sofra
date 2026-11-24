using MediatR;
using Sofra.Application.Common.Interfaces;
using Sofra.Application.Common.Pagination;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Orders.Dtos;

namespace Sofra.Application.Features.Admin.ListOrders;

public class ListOrdersQueryHandler(IOrderRepository orders)
    : IRequestHandler<ListOrdersQuery, Result<PagedResult<OrderSummaryResponse>>>
{
    public async Task<Result<PagedResult<OrderSummaryResponse>>> Handle(ListOrdersQuery request, CancellationToken cancellationToken)
    {
        return await orders.ListForAdminAsync(request.Filter, cancellationToken);
    }
}
