using Sofra.Application.Features.Carts.Dtos;

namespace Sofra.Application.Common.Interfaces;

public interface IFoodItemRepository
{
    Task<FoodItemForCartResponse?> FindForCartAsync(Guid id, CancellationToken cancellationToken = default);
}
