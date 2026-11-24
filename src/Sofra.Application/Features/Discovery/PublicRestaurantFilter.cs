namespace Sofra.Application.Features.Discovery;

/// <summary>Filter for the public restaurant discovery list (FR-06, FR-08, FR-09).</summary>
/// <param name="Search">Case-insensitive match on the restaurant name.</param>
/// <param name="CategoryId">Only restaurants that have a menu type under this platform category.</param>
/// <param name="AcceptingOrders">Only restaurants currently accepting orders.</param>
public record PublicRestaurantFilter(
    string? Search,
    Guid? CategoryId,
    bool? AcceptingOrders,
    int PageNumber = 1,
    int PageSize = 10);
