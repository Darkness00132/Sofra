using MediatR;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Restaurants.Dtos;

namespace Sofra.Application.Features.Restaurants.SetAcceptingOrders;

/// <summary>Toggles whether the signed-in owner's restaurant accepts new orders (FR-17).</summary>
public record SetAcceptingOrdersCommand(bool AcceptingOrders) : IRequest<Result<MyRestaurantResponse>>;
