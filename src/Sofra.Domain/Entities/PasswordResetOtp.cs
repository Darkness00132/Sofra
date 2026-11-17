using System.Security.Cryptography;
using System.Text;
using Sofra.Domain.Common;
using Sofra.Domain.Exceptions;

namespace Sofra.Domain.Entities;

/// <summary>
/// A one-time password reset code emailed to a user. Codes are stored hashed, expire after
/// a short window, and are single use — issuing a new code consumes every still-active one.
/// </summary>
public class PasswordResetOtp : Entity
{
    public Guid UserId { get; private set; }

    public AppUser User { get; private set; } = null!;

    public string CodeHash { get; private set; } = string.Empty;

    public DateTime ExpiresAt { get; private set; }

    public DateTime? UsedAt { get; private set; }

    public DateTime CreatedAt { get; private set; }

    private PasswordResetOtp() { }

    public static PasswordResetOtp Create(Guid userId, string codeHash, DateTime expiresAt)
    {
        if (string.IsNullOrWhiteSpace(codeHash))
            throw new DomainException("The reset code hash is required.");

        if (expiresAt <= DateTime.UtcNow)
            throw new DomainException("The reset code expiry must be in the future.");

        return new PasswordResetOtp
        {
            UserId = userId,
            CodeHash = codeHash,
            ExpiresAt = expiresAt,
            CreatedAt = DateTime.UtcNow,
        };
    }

    public void MarkUsed()
    {
        if (!IsActive)
            throw new DomainException("This reset code was already used or has expired.");

        UsedAt = DateTime.UtcNow;
    }

    public bool IsActive => UsedAt is null && DateTime.UtcNow < ExpiresAt;

    /// <summary>True when the stored hash belongs to the given plain code.</summary>
    public bool Matches(string codeHash) => CodeHash == codeHash;

    /// <summary>SHA-256 hash used to store and compare codes without keeping them in plain text.</summary>
    public static string Hash(string code) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(code)));
}
