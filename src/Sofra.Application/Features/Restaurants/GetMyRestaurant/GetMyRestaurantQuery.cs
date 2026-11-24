using MediatR;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Restaurants.Dtos;

namespace Sofra.Application.Features.Restaurants.GetMyRestaurant;

/// <summary>Returns the signed-in owner's restaurant. Fails with NotFound until one is approved.</summary>
public record GetMyRestaurantQuery : IRequest<Result<MyRestaurantResponse>>;
