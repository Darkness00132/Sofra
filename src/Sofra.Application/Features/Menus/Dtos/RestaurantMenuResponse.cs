namespace Sofra.Application.Features.Menus.Dtos;

/// <summary>A restaurant's public menu — available items only.</summary>
public record RestaurantMenuResponse(
    Guid RestaurantId,
    string RestaurantName,
    IReadOnlyList<MenuTypeResponse> MenuTypes);
