namespace Sofra.Application.Features.Discovery.Dtos;

/// <summary>The public view of an approved restaurant.</summary>
public record PublicRestaurantResponse(
    Guid Id,
    string Name,
    string Address,
    string Phone,
    string Description,
    bool AcceptingOrders,
    string? ImageUrl);
