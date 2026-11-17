using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Sofra.Application.Common.Interfaces;
using Sofra.Domain.Entities;

namespace Sofra.Infrastructure.Authentication;

/// <summary>
/// Signs short-lived JWT access tokens (subject, email and role claims) and issues
/// single-use refresh tokens that are persisted for later revocation.
/// </summary>
public class JwtTokenProvider(
    UserManager<AppUser> userManager,
    IOptions<JwtOptions> options,
    IRepository<RefreshToken> refreshTokens,
    IUnitOfWork unitOfWork) : ITokenProvider
{
    public async Task<TokenPair> GenerateTokensAsync(AppUser user, CancellationToken cancellationToken = default)
    {
        var accessTokenExpiresAtUtc = DateTime.UtcNow.AddMinutes(options.Value.AccessTokenExpiryMinutes);
        var accessToken = await GenerateAccessToken(user, accessTokenExpiresAtUtc);
        var refreshToken = GenerateRefreshTokenValue();

        await refreshTokens.AddAsync(
            RefreshToken.Create(user.Id, refreshToken, DateTime.UtcNow.AddDays(options.Value.RefreshTokenExpiryDays)),
            cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new TokenPair(accessToken, refreshToken, accessTokenExpiresAtUtc);
    }

    private async Task<string> GenerateAccessToken(AppUser user, DateTime expiresAtUtc)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        foreach (var role in await userManager.GetRolesAsync(user))
            claims.Add(new Claim(ClaimTypes.Role, role));

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Value.SigningKey));
        var token = new JwtSecurityToken(
            issuer: options.Value.Issuer,
            audience: options.Value.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expiresAtUtc,
            signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateRefreshTokenValue() =>
        Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
}
