using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Sofra.Domain.Common;
using System.Security.Claims;

namespace Sofra.Infrastructure.Persistent.Interceptors;

public class AuditableEntityInterceptor(TimeProvider timeProvider, IHttpContextAccessor httpContextAccessor)
    : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> originalResult)
    {
        Apply(eventData.Context);
        return base.SavingChanges(eventData, originalResult);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> originalResult, CancellationToken cancellationToken = default)
    {
        Apply(eventData.Context);
        return base.SavingChangesAsync(eventData, originalResult, cancellationToken);
    }

    private void Apply(DbContext? context)
    {
        if (context is null) return;

        var now = timeProvider.GetUtcNow();
        var userId = CurrentUserId();

        foreach (var entry in context.ChangeTracker.Entries<AuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.LastModifiedAt = now;
                entry.Entity.ModifiedBy = userId;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.LastModifiedAt = now;
                entry.Entity.ModifiedBy = userId;
            }
        }
    }

    private Guid? CurrentUserId()
    {
        var user = httpContextAccessor.HttpContext?.User;
        if (user?.Identity?.IsAuthenticated != true) return null;

        var id = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(id, out var userId) ? userId : null;
    }
}
