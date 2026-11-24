using MediatR;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Orders.Dtos;

namespace Sofra.Application.Features.Orders.GetOrderDetails;

/// <summary>Full details of one of the signed-in customer's own orders (FR-36, FR-43, BR-10).</summary>
public record GetOrderDetailsQuery(Guid OrderId) : IRequest<Result<OrderDetailsResponse>>;
