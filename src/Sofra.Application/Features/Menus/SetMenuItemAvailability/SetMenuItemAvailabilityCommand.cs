using MediatR;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Menus.Dtos;

namespace Sofra.Application.Features.Menus.SetMenuItemAvailability;

/// <summary>Marks one of the owner's food items available or unavailable (FR-16, BR-05).</summary>
public record SetMenuItemAvailabilityCommand(Guid FoodItemId, bool Available) : IRequest<Result<MenuItemResponse>>;
