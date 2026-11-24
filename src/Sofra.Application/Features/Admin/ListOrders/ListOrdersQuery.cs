using MediatR;
using Sofra.Application.Common.Pagination;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Orders.Dtos;
using Sofra.Application.Features.Orders;

namespace Sofra.Application.Features.Admin.ListOrders;

/// <summary>Administrator list of all platform orders with filters and paging (FR-45, FR-51).</summary>
public record ListOrdersQuery(AdminOrderListFilter Filter) : IRequest<Result<PagedResult<OrderSummaryResponse>>>;
