using Mapster;
using MediatR;
using Sofra.Application.Common.Interfaces;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.FoodCategories.Dtos;
using Sofra.Domain.Exceptions;

namespace Sofra.Application.Features.FoodCategories.UpdateCategory;

public class UpdateCategoryCommandHandler(
    IFoodCategoryRepository categories,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateCategoryCommand, Result<FoodCategoryResponse>>
{
    public async Task<Result<FoodCategoryResponse>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await categories.FindAsync(request.CategoryId, cancellationToken);
        if (category is null)
            return Result<FoodCategoryResponse>.Failure(Error.NotFound(
                "FOOD_CATEGORY_NOT_FOUND",
                "The food category was not found."));

        try
        {
            category.UpdateInfo(request.Name, request.Description);
        }
        catch (DomainException ex)
        {
            return Result<FoodCategoryResponse>.Failure(Error.BusinessRule("CATEGORY_INFO_INVALID", ex.Message));
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return category.Adapt<FoodCategoryResponse>();
    }
}
