using MediatR;
using Sofra.Application.Common.Pagination;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Restaurants.Dtos;

namespace Sofra.Application.Features.Restaurants.Admin.ListApplications;

/// <summary>Administrator list of restaurant applications with filters and paging.</summary>
public record ListApplicationsQuery(ApplicationListFilter Filter) : IRequest<Result<PagedResult<ApplicationResponse>>>;
