using Microsoft.EntityFrameworkCore;
using Sofra.Application.Common.Interfaces;
using Sofra.Application.Features.Menus.Dtos;
using Sofra.Domain.Entities;
using Sofra.Domain.Enums;
using Sofra.Infrastructure.Persistent;

namespace Sofra.Infrastructure.Repositories;

public class MenuRepository(SofraDbContext context) : IMenuRepository
{
    public async Task<MyMenuResponse> GetMyMenuAsync(Guid restaurantId, CancellationToken cancellationToken = default)
    {
        var types = await context.FoodTypes
            .AsNoTracking()
            .Where(t => t.RestaurantId == restaurantId)
            .Include(t => t.FoodItems)
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken);

        var response = types.Select(t => new MenuTypeResponse(
            t.Id,
            t.FoodCategoryId,
            t.Name,
            t.Description,
            t.FoodItems
                .OrderBy(i => i.Name)
                .Select(i => new MenuItemResponse(i.Id, i.Name, i.Description, i.Price, i.Availability.ToString(), i.IsAvailable, i.ImageUrl))
                .ToList())).ToList();

        return new MyMenuResponse(restaurantId, response);
    }

    public async Task<IReadOnlyList<MenuTypeResponse>> GetPublicMenuTypesAsync(Guid restaurantId, CancellationToken cancellationToken = default)
    {
        var types = await context.FoodTypes
            .AsNoTracking()
            .Where(t => t.RestaurantId == restaurantId)
            .Include(t => t.FoodItems)
            .ToListAsync(cancellationToken);

        return types
            .Where(t => t.FoodItems.Any(i => i.Availability == FoodAvailability.Available))
            .OrderBy(t => t.Name)
            .Select(t => new MenuTypeResponse(
                t.Id,
                t.FoodCategoryId,
                t.Name,
                t.Description,
                t.FoodItems
                    .Where(i => i.Availability == FoodAvailability.Available)
                    .OrderBy(i => i.Name)
                    .Select(i => new MenuItemResponse(i.Id, i.Name, i.Description, i.Price, i.Availability.ToString(), i.IsAvailable, i.ImageUrl))
                    .ToList()))
            .ToList();
    }
}
