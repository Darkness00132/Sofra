using MediatR;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Orders.Dtos;

namespace Sofra.Application.Features.Admin.GetOrderDetails;

/// <summary>Full details of any order, for administrator oversight (FR-45, FR-51).</summary>
public record GetOrderDetailsQuery(Guid OrderId) : IRequest<Result<OrderDetailsResponse>>;
