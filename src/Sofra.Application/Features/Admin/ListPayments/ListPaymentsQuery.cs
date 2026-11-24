using MediatR;
using Sofra.Application.Common.Pagination;
using Sofra.Application.Common.Results;
using Sofra.Domain.Entities;

namespace Sofra.Application.Features.Admin.ListPayments;

/// <summary>Administrator list of payment records with filters and paging.</summary>
public record ListPaymentsQuery(PaymentListFilter Filter) : IRequest<Result<PagedResult<PaymentResponse>>>;
