using MediatR;
using Sofra.Application.Common.Pagination;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Orders.Dtos;

namespace Sofra.Application.Features.Orders.ListMyOrders;

/// <summary>Lists the signed-in customer's own orders with filters and paging (FR-36, FR-43).</summary>
public record ListMyOrdersQuery(MyOrdersFilter Filter) : IRequest<Result<PagedResult<OrderSummaryResponse>>>;
