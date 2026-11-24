using MediatR;
using Sofra.Application.Common.Pagination;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.FoodCategories.Dtos;

namespace Sofra.Application.Features.FoodCategories.ListCategories;

/// <summary>Public catalog of platform food categories, used for discovery filters and menu setup.</summary>
public record ListCategoriesQuery(FoodCategoryFilter Filter) : IRequest<Result<PagedResult<FoodCategoryResponse>>>;
