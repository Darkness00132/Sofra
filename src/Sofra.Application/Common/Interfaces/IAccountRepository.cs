using Sofra.Application.Common.Pagination;
using Sofra.Application.Features.Admin.ListAccounts;

namespace Sofra.Application.Common.Interfaces;

/// <summary>
/// Administrator account queries. Identity users are not domain entities, so their paged,
/// role-filtered list and role counts need their own queries instead of IRepository&lt;T&gt;.
/// </summary>
public interface IAccountRepository
{
    Task<PagedResult<AdminAccountResponse>> ListAsync(AdminAccountFilter filter, CancellationToken cancellationToken = default);

    Task<int> CountInRoleAsync(string role, CancellationToken cancellationToken = default);

    /// <summary>The account's email address, or null when the account no longer exists.</summary>
    Task<string?> GetEmailAsync(Guid userId, CancellationToken cancellationToken = default);
}
