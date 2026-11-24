using Sofra.Domain.Entities;

namespace Sofra.Application.Common.Interfaces;

/// <summary>
/// Refresh-token reads that need the owning user in the same query, so token refresh does not
/// query the user again after loading the token.
/// </summary>
public interface IRefreshTokenRepository : IRepository<RefreshToken>
{
    /// <summary>Finds one token with its <see cref="AppUser"/> loaded.</summary>
    Task<RefreshToken?> FindWithUserAsync(string token, CancellationToken cancellationToken = default);
}
