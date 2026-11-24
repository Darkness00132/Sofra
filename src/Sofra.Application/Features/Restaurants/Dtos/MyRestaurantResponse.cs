namespace Sofra.Application.Features.Restaurants.Dtos;

/// <summary>The signed-in owner's own restaurant.</summary>
public record MyRestaurantResponse(
    Guid Id,
    string Name,
    string Address,
    string Phone,
    string Description,
    string Status,
    bool AcceptingOrders,
    string? ImageUrl);
