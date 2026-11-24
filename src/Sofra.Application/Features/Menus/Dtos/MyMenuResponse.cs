namespace Sofra.Application.Features.Menus.Dtos;

/// <summary>The owner's full menu — includes unavailable items (management view).</summary>
public record MyMenuResponse(
    Guid RestaurantId,
    IReadOnlyList<MenuTypeResponse> MenuTypes);
