using MediatR;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Restaurants.Dtos;

namespace Sofra.Application.Features.Restaurants.RemoveRestaurantImage;

/// <summary>Removes the owner's restaurant picture from the menu and from file storage.</summary>
public record RemoveRestaurantImageCommand : IRequest<Result<MyRestaurantResponse>>;
