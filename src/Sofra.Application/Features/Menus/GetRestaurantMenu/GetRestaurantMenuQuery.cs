using MediatR;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Menus.Dtos;

namespace Sofra.Application.Features.Menus.GetRestaurantMenu;

/// <summary>Public view of one approved restaurant's menu — available items only (FR-07).</summary>
public record GetRestaurantMenuQuery(Guid RestaurantId) : IRequest<Result<RestaurantMenuResponse>>;
