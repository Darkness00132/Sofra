using Sofra.Application.Constants;
namespace Sofra.Application.Features.Admin.ListAccounts;

/// <summary>A user account as seen by administrators.</summary>
public record AdminAccountResponse(
    Guid Id,
    string Email,
    string? PhoneNumber,
    bool Locked,
    IReadOnlyList<string> Roles);
