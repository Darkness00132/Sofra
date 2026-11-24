using Sofra.Application.Features.Carts.Dtos;
using Sofra.Domain.Entities;

namespace Sofra.Application.Common.Interfaces;

/// <summary>Cart repository — the cart views need items with their food details and the restaurant.</summary>
public interface ICartRepository : IRepository<Cart>
{
    /// <summary>Finds the user's cart with its items and food details loaded (for domain mutations); null when the user has no cart.</summary>
    Task<Cart?> FindForUserAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>The user's cart as a read model, projected in SQL (items + totals); null when the user has no cart.</summary>
    Task<CartResponse?> GetForUserAsync(Guid userId, CancellationToken cancellationToken = default);
}
