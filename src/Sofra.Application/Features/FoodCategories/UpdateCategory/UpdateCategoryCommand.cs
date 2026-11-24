using MediatR;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.FoodCategories.Dtos;

namespace Sofra.Application.Features.FoodCategories.UpdateCategory;

/// <summary>Updates a platform-level food category (administrator).</summary>
public record UpdateCategoryCommand(Guid CategoryId, string Name, string? Description) : IRequest<Result<FoodCategoryResponse>>;
