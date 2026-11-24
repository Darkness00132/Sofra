using MediatR;
using Sofra.Application.Common.Results;

namespace Sofra.Application.Features.Menus.RemoveMenuItem;

/// <summary>Removes one of the owner's food items. Blocked when past orders reference it (BR-04, FR-46).</summary>
public record RemoveMenuItemCommand(Guid FoodItemId) : IRequest<Result>;
