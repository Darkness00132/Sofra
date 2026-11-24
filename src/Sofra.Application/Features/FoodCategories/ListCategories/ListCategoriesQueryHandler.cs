using MediatR;
using Sofra.Application.Common.Interfaces;
using Sofra.Application.Common.Pagination;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.FoodCategories.Dtos;

namespace Sofra.Application.Features.FoodCategories.ListCategories;

public class ListCategoriesQueryHandler(IFoodCategoryRepository categories)
    : IRequestHandler<ListCategoriesQuery, Result<PagedResult<FoodCategoryResponse>>>
{
    public async Task<Result<PagedResult<FoodCategoryResponse>>> Handle(ListCategoriesQuery request, CancellationToken cancellationToken)
    {
        var page = await categories.ListAsync(request.Filter, cancellationToken);
        return page;
    }
}
