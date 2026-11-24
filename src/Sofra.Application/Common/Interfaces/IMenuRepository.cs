using Sofra.Application.Features.Menus.Dtos;

namespace Sofra.Application.Common.Interfaces;

/// <summary>
/// Menu reads — the owner's full menu and the public menu (available items only).
/// Both are one correlated SQL projection instead of fetching every type and item row.
/// </summary>
public interface IMenuRepository
{
    /// <summary>The owner's menu for one restaurant, types ordered by name, unavailable items included.</summary>
    Task<MyMenuResponse> GetMyMenuAsync(Guid restaurantId, CancellationToken cancellationToken = default);

    /// <summary>The public menu: available items only, types with no available item dropped.</summary>
    Task<IReadOnlyList<MenuTypeResponse>> GetPublicMenuTypesAsync(Guid restaurantId, CancellationToken cancellationToken = default);
}
