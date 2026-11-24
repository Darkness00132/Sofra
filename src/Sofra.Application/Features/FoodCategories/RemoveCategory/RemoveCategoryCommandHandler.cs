using MediatR;
using Sofra.Application.Common.Interfaces;
using Sofra.Application.Common.Results;
using Sofra.Domain.Entities;

namespace Sofra.Application.Features.FoodCategories.RemoveCategory;

public class RemoveCategoryCommandHandler(
    IFoodCategoryRepository categories,
    IRepository<FoodType> menuTypes,
    IUnitOfWork unitOfWork) : IRequestHandler<RemoveCategoryCommand, Result>
{
    public async Task<Result> Handle(RemoveCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await categories.FindAsync(request.CategoryId, cancellationToken);
        if (category is null)
            return Result.Failure(Error.NotFound(
                "FOOD_CATEGORY_NOT_FOUND",
                "The food category was not found."));

        if (await menuTypes.ExistsAsync(t => t.FoodCategoryId == request.CategoryId, cancellationToken))
            return Result.Failure(Error.Conflict(
                "CATEGORY_IN_USE",
                "Menu types still use this category; remove them first."));

        categories.Delete(category);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
