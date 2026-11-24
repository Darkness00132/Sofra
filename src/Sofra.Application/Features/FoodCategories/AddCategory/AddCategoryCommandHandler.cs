using Mapster;
using MediatR;
using Sofra.Application.Common.Interfaces;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.FoodCategories.Dtos;
using Sofra.Domain.Entities;

namespace Sofra.Application.Features.FoodCategories.AddCategory;

public class AddCategoryCommandHandler(
    IFoodCategoryRepository categories,
    IUnitOfWork unitOfWork) : IRequestHandler<AddCategoryCommand, Result<FoodCategoryResponse>>
{
    public async Task<Result<FoodCategoryResponse>> Handle(AddCategoryCommand request, CancellationToken cancellationToken)
    {
        if (await categories.ExistsAsync(c => c.Name == request.Name, cancellationToken))
            return Result<FoodCategoryResponse>.Failure(Error.Conflict(
                "CATEGORY_ALREADY_EXISTS",
                "A food category with this name already exists."));

        var category = FoodCategory.Create(request.Name, request.Description);

        await categories.AddAsync(category, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return category.Adapt<FoodCategoryResponse>();
    }
}
