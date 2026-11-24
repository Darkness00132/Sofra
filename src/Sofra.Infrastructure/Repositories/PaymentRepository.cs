using Mapster;
using Microsoft.EntityFrameworkCore;
using Sofra.Application.Common.Interfaces;
using Sofra.Application.Common.Pagination;
using Sofra.Application.Features.Admin.ListPayments;
using Sofra.Domain.Entities;
using Sofra.Infrastructure.Repositories.Extensions;
using Sofra.Infrastructure.Persistent;

namespace Sofra.Infrastructure.Repositories;

public class PaymentRepository(SofraDbContext context) : Repository<Payment>(context), IPaymentRepository
{
    public async Task<PagedResult<PaymentResponse>> ListAsync(PaymentListFilter filter, CancellationToken cancellationToken = default)
    {
        var query = Context.Payments.ApplyFilter(filter).ProjectToType<PaymentResponse>(TypeAdapterConfig.GlobalSettings);
        return await this.ToPagedResultAsync(query, filter.PageNumber, filter.PageSize, cancellationToken);
    }
}
