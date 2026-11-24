using MediatR;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Restaurants.Dtos;

namespace Sofra.Application.Features.Restaurants.UpdateMyRestaurant;

/// <summary>Updates the signed-in owner's restaurant information (FR-12).</summary>
public record UpdateMyRestaurantCommand(
    string Name,
    string Address,
    string Phone,
    string Description) : IRequest<Result<MyRestaurantResponse>>;
