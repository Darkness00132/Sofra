using MediatR;
using Sofra.Application.Common.Pagination;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Restaurants.Dtos;

namespace Sofra.Application.Features.Restaurants.Admin.ListRestaurants;

/// <summary>Administrator list of restaurants with filters and paging.</summary>
public record ListRestaurantsQuery(RestaurantListFilter Filter) : IRequest<Result<PagedResult<RestaurantResponse>>>;
