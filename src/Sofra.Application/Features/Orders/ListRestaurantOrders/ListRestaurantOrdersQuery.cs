using MediatR;
using Sofra.Application.Common.Pagination;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Orders.Dtos;

namespace Sofra.Application.Features.Orders.ListRestaurantOrders;

/// <summary>Lists the signed-in owner's restaurant orders — status filters cover incoming and history (FR-32, FR-44).</summary>
public record ListRestaurantOrdersQuery(RestaurantOrdersFilter Filter) : IRequest<Result<PagedResult<OrderSummaryResponse>>>;
