using Microsoft.EntityFrameworkCore;
using Sofra.Application.Common.Interfaces;
using Sofra.Application.Common.Pagination;
using Sofra.Application.Features.Admin.ListAccounts;
using Sofra.Domain.Entities;
using Sofra.Infrastructure.Repositories.Extensions;
using Sofra.Infrastructure.Persistent;

namespace Sofra.Infrastructure.Repositories;

public class AccountRepository(SofraDbContext context) : Repository<AppUser>(context), IAccountRepository
{
    public async Task<PagedResult<AdminAccountResponse>> ListAsync(
        AdminAccountFilter filter,
        CancellationToken cancellationToken = default)
    {
        var query = context.Users.AsQueryable();

        if (filter.Role is not null)
        {
            var roleIds = context.Roles.Where(r => r.Name == filter.Role).Select(r => r.Id);
            var userIds = context.UserRoles.Where(ur => roleIds.Contains(ur.RoleId)).Select(ur => ur.UserId);
            query = query.Where(u => userIds.Contains(u.Id));
        }

        if (!string.IsNullOrWhiteSpace(filter.Search))
            query = query.Where(u => u.Email != null && u.Email.Contains(filter.Search));

        var page = await this.ToPagedResultAsync(
            query.OrderBy(u => u.Email),
            filter.PageNumber, filter.PageSize, cancellationToken);

        var pageUserIds = page.Items.Select(u => u.Id).ToList();

        // One query for every account on the page instead of GetRolesAsync per row.
        var userRoles = await context.UserRoles
            .Join(context.Roles,
                userRole => userRole.RoleId,
                role => role.Id,
                (userRole, role) => new { userRole.UserId, role.Name })
            .Where(x => pageUserIds.Contains(x.UserId) && x.Name != null)
            .ToListAsync(cancellationToken);

        var rolesByUser = userRoles
            .GroupBy(x => x.UserId)
            .ToDictionary(
                group => group.Key,
                group => (IReadOnlyList<string>)group
                    .Select(x => x.Name!)
                    .OrderBy(name => name, StringComparer.Ordinal)
                    .ToList());

        var items = page.Items
            .Select(user => new AdminAccountResponse(
                user.Id,
                user.Email ?? string.Empty,
                user.PhoneNumber,
                user.LockoutEnd is not null && user.LockoutEnd > DateTimeOffset.UtcNow,
                rolesByUser.TryGetValue(user.Id, out var roles) ? roles : []))
            .ToList();

        return new PagedResult<AdminAccountResponse>(items, page.PageNumber, page.PageSize, page.TotalCount);
    }

    public Task<int> CountInRoleAsync(string role, CancellationToken cancellationToken = default)
    {
        var roleIds = context.Roles.Where(r => r.Name == role).Select(r => r.Id);
        var userIds = context.UserRoles.Where(ur => roleIds.Contains(ur.RoleId)).Select(ur => ur.UserId);

        return userIds.Distinct().CountAsync(cancellationToken);
    }

    public Task<string?> GetEmailAsync(Guid userId, CancellationToken cancellationToken = default) =>
        context.Users.Where(u => u.Id == userId).Select(u => u.Email).FirstOrDefaultAsync(cancellationToken);
}
