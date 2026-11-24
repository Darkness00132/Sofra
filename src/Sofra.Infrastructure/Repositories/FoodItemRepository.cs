using Mapster;
using Microsoft.EntityFrameworkCore;
using Sofra.Application.Common.Interfaces;
using Sofra.Application.Features.Carts.Dtos;
using Sofra.Infrastructure.Persistent;

namespace Sofra.Infrastructure.Repositories;

public class FoodItemRepository(SofraDbContext context) : IFoodItemRepository
{
    public Task<FoodItemForCartResponse?> FindForCartAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.FoodItems.Where(i => i.Id == id).ProjectToType<FoodItemForCartResponse>(TypeAdapterConfig.GlobalSettings).FirstOrDefaultAsync(cancellationToken);
}
