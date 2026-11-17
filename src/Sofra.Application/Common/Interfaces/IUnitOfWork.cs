namespace Sofra.Application.Common.Interfaces;

/// <summary>
/// Saves changes and controls transactions. All repositories share the same underlying
/// <c>SofraDbContext</c> within a request, so one <see cref="SaveChangesAsync"/> persists
/// every change made through every repository. Implemented in Infrastructure.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>Persists every tracked change from all repositories of this request.</summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>Starts a database transaction.</summary>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>Commits the current transaction.</summary>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>Rolls back the current transaction.</summary>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
