using Sofra.Domain.Enums;

namespace Sofra.Application.Features.Restaurants.Admin.ListApplications;

/// <summary>Filter for the administrator's application list.</summary>
/// <param name="Status">Only applications in this status; null = all.</param>
/// <param name="Search">Case-insensitive match on the restaurant name.</param>
public record ApplicationListFilter(
    RestaurantStatus? Status,
    string? Search,
    int PageNumber = 1,
    int PageSize = 10);
