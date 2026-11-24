using Sofra.Application.Constants;
using Sofra.Domain.Entities;

namespace Sofra.Application.Features.Admin.ListAccounts;

/// <summary>Filter for the administrator's account list (FR-50).</summary>
/// <param name="Role">Only accounts holding this role (Customer, Restaurant or Administrator); omit for all.</param>
/// <param name="Search">Case-insensitive match on the account email.</param>
public record AdminAccountFilter(
    string? Role,
    string? Search,
    int PageNumber = 1,
    int PageSize = 10)
{
    public bool IsValidRole() =>
        Role is null || Roles.All.Contains(Role);
}
