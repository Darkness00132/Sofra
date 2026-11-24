using Sofra.Domain.Enums;

namespace Sofra.Application.Features.Restaurants.Admin.ListRestaurants;

/// <summary>Filter for the administrator's restaurant list.</summary>
/// <param name="Status">Only restaurants in this status; null = all.</param>
/// <param name="Search">Case-insensitive match on the restaurant name.</param>
public record RestaurantListFilter(
    RestaurantStatus? Status,
    string? Search,
    int PageNumber = 1,
    int PageSize = 10);
