using Sofra.Domain.Entities;

namespace Sofra.Application.Common.Interfaces;

/// <summary>
/// Creates a session's tokens: a signed JWT access token and a persisted, single-use
/// refresh token. Implemented in Infrastructure (JWT signing lives there).
/// </summary>
public interface ITokenProvider
{
    /// <summary>
    /// Generates an access token for the user and persists a new refresh token.
    /// </summary>
    Task<TokenPair> GenerateTokensAsync(AppUser user, CancellationToken cancellationToken = default);
}
