using Sofra.Application.Common.Interfaces;
using Sofra.Infrastructure.Persistent;

namespace Sofra.Infrastructure.Repositories;

/// <summary>
/// Saves and controls transactions over <see cref="SofraDbContext"/>. All repositories of a
/// request share this same context, so one save persists every repository's changes.
/// </summary>
public class UnitOfWork(SofraDbContext context) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default) =>
        await context.Database.BeginTransactionAsync(cancellationToken);

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default) =>
        await context.Database.CommitTransactionAsync(cancellationToken);

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default) =>
        await context.Database.RollbackTransactionAsync(cancellationToken);
}
