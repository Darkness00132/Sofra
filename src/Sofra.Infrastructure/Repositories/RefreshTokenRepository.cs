using Microsoft.EntityFrameworkCore;
using Sofra.Application.Common.Interfaces;
using Sofra.Domain.Entities;
using Sofra.Infrastructure.Persistent;

namespace Sofra.Infrastructure.Repositories;

public class RefreshTokenRepository(SofraDbContext context)
    : Repository<RefreshToken>(context), IRefreshTokenRepository
{
    /// <summary>Tracked read with the owning user, so refresh does not load the user separately.</summary>
    public Task<RefreshToken?> FindWithUserAsync(string token, CancellationToken cancellationToken = default) =>
        Context.RefreshTokens
            .Include(t => t.User)
            .SingleOrDefaultAsync(t => t.Token == token, cancellationToken);
}
