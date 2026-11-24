namespace Sofra.Application.Features.Menus.Dtos;

/// <summary>A restaurant's menu type (grouping under a platform category) with its food items.</summary>
public record MenuTypeResponse(
    Guid Id,
    Guid FoodCategoryId,
    string Name,
    string? Description,
    IReadOnlyList<MenuItemResponse> Items);
