using Mapster;
using Sofra.Application.Features.Menus.Dtos;
using Sofra.Domain.Entities;
using Sofra.Domain.Enums;

namespace Sofra.Application.Features.Menus.Mapping;

/// <summary>
/// Menu → response maps. The nested <c>FoodItems → Items</c> pair is registered so Mapster
/// derives the collection (it does not reliably expand collection members into init-only
/// record properties) — and ordered by item name, matching the previous in-memory order.
/// <c>IsAvailable</c> is a computed property on the entity, so it is mapped from the stored
/// <c>Availability</c> explicitly — a bare name-match would let EF fold it to a constant.
/// <c>Availability</c> is stored through an enum but read back as a string, so the string
/// form is mapped explicitly to match the in-memory <c>Adapt</c> path.
/// </summary>
public class MenuMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<FoodType, MenuTypeResponse>()
            .Map(d => d.Items, s => s.FoodItems.OrderBy(i => i.Name));

        config.NewConfig<FoodItem, MenuItemResponse>()
            .Map(d => d.Availability, s => s.Availability.ToString())
            .Map(d => d.IsAvailable, s => s.Availability == FoodAvailability.Available);
    }
}
