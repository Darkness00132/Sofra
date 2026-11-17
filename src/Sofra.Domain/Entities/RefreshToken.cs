using Sofra.Domain.Common;
using Sofra.Domain.Exceptions;

namespace Sofra.Domain.Entities;

/// <summary>
/// A refresh token issued for a signed-in user. Single use: issuing a new token pair
/// revokes the previous one; a token that is revoked or expired is no longer active.
/// </summary>
public class RefreshToken : Entity
{
    public Guid UserId { get; private set; }

    public AppUser User { get; private set; } = null!;

    public string Token { get; private set; } = string.Empty;

    public DateTime ExpiresAt { get; private set; }

    public DateTime? RevokedAt { get; private set; }

    public DateTime CreatedAt { get; private set; }

    private RefreshToken() { }

    public static RefreshToken Create(Guid userId, string token, DateTime expiresAt)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new DomainException("Refresh token value is required.");

        if (expiresAt <= DateTime.UtcNow)
            throw new DomainException("Refresh token expiry must be in the future.");

        return new RefreshToken
        {
            UserId = userId,
            Token = token,
            ExpiresAt = expiresAt,
            CreatedAt = DateTime.UtcNow,
        };
    }

    public void Revoke()
    {
        if (RevokedAt is not null)
            throw new DomainException("Refresh token is already revoked.");

        RevokedAt = DateTime.UtcNow;
    }

    public bool IsActive => RevokedAt is null && DateTime.UtcNow < ExpiresAt;
}
