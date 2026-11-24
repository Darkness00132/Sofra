namespace Sofra.Application.Features.Restaurants.Dtos;

/// <summary>A restaurant as seen by administrators.</summary>
public record RestaurantResponse(
    Guid Id,
    string Name,
    string Address,
    string Phone,
    string Description,
    string Status,
    bool AcceptingOrders,
    string OwnerEmail);
