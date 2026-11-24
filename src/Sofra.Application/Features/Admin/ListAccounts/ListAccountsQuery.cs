using MediatR;
using Sofra.Application.Common.Pagination;
using Sofra.Application.Common.Results;

namespace Sofra.Application.Features.Admin.ListAccounts;

/// <summary>Administrator list of user accounts with role filter, search and paging (FR-50).</summary>
public record ListAccountsQuery(AdminAccountFilter Filter) : IRequest<Result<PagedResult<AdminAccountResponse>>>;
