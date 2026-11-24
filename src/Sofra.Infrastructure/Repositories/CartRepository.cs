using Mapster;
using Microsoft.EntityFrameworkCore;
using Sofra.Application.Common.Interfaces;
using Sofra.Application.Features.Carts.Dtos;
using Sofra.Domain.Entities;
using Sofra.Infrastructure.Persistent;

namespace Sofra.Infrastructure.Repositories;

public class CartRepository(SofraDbContext context) : Repository<Cart>(context), ICartRepository
{
    public Task<Cart?> FindForUserAsync(Guid userId, CancellationToken cancellationToken = default) =>
        Context.Carts.Include(c => c.CartItems).ThenInclude(i => i.FoodItem).FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);

    public async Task<CartResponse?> GetForUserAsync(Guid userId, CancellationToken cancellationToken = default) =>
        await Context.Carts.Where(c => c.UserId == userId).ProjectToType<CartResponse>(TypeAdapterConfig.GlobalSettings).FirstOrDefaultAsync(cancellationToken);
}
