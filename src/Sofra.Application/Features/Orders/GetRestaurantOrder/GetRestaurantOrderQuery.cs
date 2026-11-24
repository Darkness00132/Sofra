using MediatR;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Orders.Dtos;

namespace Sofra.Application.Features.Orders.GetRestaurantOrder;

/// <summary>Full details of one of the signed-in owner's restaurant orders (BR-03, BR-10).</summary>
public record GetRestaurantOrderQuery(Guid OrderId) : IRequest<Result<OrderDetailsResponse>>;
