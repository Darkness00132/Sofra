using MediatR;
using Sofra.Application.Common.Results;

namespace Sofra.Application.Features.FoodCategories.RemoveCategory;

/// <summary>Removes a platform-level food category. Blocked while menu types still reference it.</summary>
public record RemoveCategoryCommand(Guid CategoryId) : IRequest<Result>;
