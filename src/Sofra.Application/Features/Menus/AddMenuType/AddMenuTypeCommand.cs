using MediatR;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Menus.Dtos;

namespace Sofra.Application.Features.Menus.AddMenuType;

/// <summary>Adds a menu type under a platform category to the owner's menu.</summary>
public record AddMenuTypeCommand(Guid FoodCategoryId, string Name, string? Description) : IRequest<Result<MenuTypeResponse>>;
