using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Sofra.Application.Common.Interfaces;
using Sofra.Infrastructure.Persistent;

namespace Sofra.Infrastructure.Repositories;

/// <summary>EF Core repository for one entity type over <see cref="SofraDbContext"/> — the common operations only.</summary>
public class Repository<T>(SofraDbContext context) : IRepository<T> where T : class
{
    public Task<T?> FindAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.Set<T>().FindAsync([id], cancellationToken).AsTask();

    public Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default) =>
        context.Set<T>().FirstOrDefaultAsync(predicate, cancellationToken);

    public Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default) =>
        context.Set<T>().SingleOrDefaultAsync(predicate, cancellationToken);

    public Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default) =>
        context.Set<T>().AnyAsync(predicate, cancellationToken);

    public async Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default) =>
        predicate is null
            ? await context.Set<T>().ToListAsync(cancellationToken)
            : await context.Set<T>().Where(predicate).ToListAsync(cancellationToken);

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default) =>
        await context.Set<T>().AddAsync(entity, cancellationToken);

    public void Delete(T entity) =>
        context.Set<T>().Remove(entity);
}
