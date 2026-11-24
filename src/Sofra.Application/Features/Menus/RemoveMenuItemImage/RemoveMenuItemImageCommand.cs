using MediatR;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Menus.Dtos;

namespace Sofra.Application.Features.Menus.RemoveMenuItemImage;

/// <summary>Removes the picture of one of the owner's food items from the menu and from storage.</summary>
public record RemoveMenuItemImageCommand(Guid FoodItemId) : IRequest<Result<MenuItemResponse>>;
