using MediatR;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Restaurants.Dtos;
using Sofra.Domain.Enums;

namespace Sofra.Application.Features.Restaurants.Admin.UpdateRestaurantStatus;

/// <summary>Suspends or reinstates a restaurant (BR-09: a suspended restaurant cannot accept orders).</summary>
public record UpdateRestaurantStatusCommand(
    Guid RestaurantId,
    RestaurantStatus Status) : IRequest<Result<RestaurantResponse>>;
