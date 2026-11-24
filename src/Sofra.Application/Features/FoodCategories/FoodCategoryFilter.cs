namespace Sofra.Application.Features.FoodCategories;

/// <summary>Filter for the public food-category catalog.</summary>
/// <param name="Search">Case-insensitive match on the category name.</param>
public record FoodCategoryFilter(
    string? Search,
    int PageNumber = 1,
    int PageSize = 50);
