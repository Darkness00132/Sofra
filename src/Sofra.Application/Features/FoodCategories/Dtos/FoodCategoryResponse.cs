namespace Sofra.Application.Features.FoodCategories.Dtos;

/// <summary>A platform-level food category (e.g. Burger, Pizza).</summary>
public record FoodCategoryResponse(
    Guid Id,
    string Name,
    string? Description);
