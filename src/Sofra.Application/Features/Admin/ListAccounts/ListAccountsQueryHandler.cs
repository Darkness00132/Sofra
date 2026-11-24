using MediatR;
using Sofra.Application.Common.Interfaces;
using Sofra.Application.Common.Pagination;
using Sofra.Application.Common.Results;

namespace Sofra.Application.Features.Admin.ListAccounts;

public class ListAccountsQueryHandler(
    IAccountRepository accounts,
    ICurrentUser currentUser) : IRequestHandler<ListAccountsQuery, Result<PagedResult<AdminAccountResponse>>>
{
    public async Task<Result<PagedResult<AdminAccountResponse>>> Handle(ListAccountsQuery request, CancellationToken cancellationToken)
    {
        var adminId = currentUser.GetUserId();

        return await accounts.ListAsync(request.Filter, cancellationToken);
    }
}
