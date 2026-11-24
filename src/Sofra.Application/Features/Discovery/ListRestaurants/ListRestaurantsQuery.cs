using MediatR;
using Sofra.Application.Common.Pagination;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Discovery.Dtos;
using Sofra.Application.Features.Restaurants.Admin.ListRestaurants;

namespace Sofra.Application.Features.Discovery.ListRestaurants;

/// <summary>Public browsing of approved restaurants (FR-06, FR-08, FR-09, FR-10).</summary>
public record ListRestaurantsQuery(PublicRestaurantFilter Filter) : IRequest<Result<PagedResult<PublicRestaurantResponse>>>;
