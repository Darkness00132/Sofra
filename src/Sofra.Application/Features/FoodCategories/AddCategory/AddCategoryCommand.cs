using MediatR;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.FoodCategories.Dtos;

namespace Sofra.Application.Features.FoodCategories.AddCategory;

/// <summary>Adds a platform-level food category (administrator).</summary>
public record AddCategoryCommand(string Name, string? Description) : IRequest<Result<FoodCategoryResponse>>;
