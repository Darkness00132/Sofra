using Sofra.Application.Common.Pagination;
using System.Linq.Expressions;

namespace Sofra.Application.Common.Interfaces;

/// <summary>
/// Repository over EF Core for one entity type — only the common operations. Saving and
/// transactions live in <see cref="IUnitOfWork"/>; when an entity needs more than these
/// operations, give it its own repository interface inheriting this one.
/// Implemented in Infrastructure by <c>Repository&lt;T&gt;</c>.
/// </summary>
public interface IRepository<T> where T : class
{
    /// <summary>Finds one entity by its primary key (Guid Id).</summary>
    Task<T?> FindAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Finds the first entity matching the predicate, or null.</summary>
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>Finds the single entity matching the predicate, or null — throws when several match.</summary>
    Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>True when at least one entity matches the predicate.</summary>
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>Lists all entities, or only those matching the predicate.</summary>
    Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default);

    /// <summary>Filters stay in entity-specific repositories; this handles only the common paging.</summary>
    Task<PagedResult<TDto>> ToPagedResultAsync<TDto>(IQueryable<TDto> query, int pageNumber, int pageSize, CancellationToken cancellationToken = default);

    /// <summary>Marks a new entity for insertion (persisted by <see cref="IUnitOfWork.SaveChangesAsync"/>).</summary>
    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>Marks an entity for deletion (persisted by <see cref="IUnitOfWork.SaveChangesAsync"/>).</summary>
    void Delete(T entity);
}
