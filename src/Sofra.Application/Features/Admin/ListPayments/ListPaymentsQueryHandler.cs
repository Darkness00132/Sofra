using MediatR;
using Sofra.Application.Common.Interfaces;
using Sofra.Application.Common.Pagination;
using Sofra.Application.Common.Results;

namespace Sofra.Application.Features.Admin.ListPayments;

public class ListPaymentsQueryHandler(IPaymentRepository payments)
    : IRequestHandler<ListPaymentsQuery, Result<PagedResult<PaymentResponse>>>
{
    public async Task<Result<PagedResult<PaymentResponse>>> Handle(ListPaymentsQuery request, CancellationToken cancellationToken)
    {
        var page = await payments.ListAsync(request.Filter, cancellationToken);
        return page;
    }
}
