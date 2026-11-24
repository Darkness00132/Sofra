using MediatR;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Menus.Dtos;

namespace Sofra.Application.Features.Menus.UpdateMenuItem;

/// <summary>Updates one of the owner's food items (FR-14, FR-15). Price changes never affect past orders (BR-04).</summary>
public record UpdateMenuItemCommand(Guid FoodItemId, string Name, string Description, decimal Price) : IRequest<Result<MenuItemResponse>>;
