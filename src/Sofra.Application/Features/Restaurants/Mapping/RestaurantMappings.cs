using Mapster;
using Sofra.Application.Features.Discovery.Dtos;
using Sofra.Application.Features.Restaurants.Dtos;
using Sofra.Domain.Entities;

namespace Sofra.Application.Features.Restaurants.Mapping;

/// <summary>
/// Restaurant → response maps: everything else is same-name, but the entity's flag is named
/// IsAcceptingOrders while the API contract calls it acceptingOrders.
/// </summary>
public class RestaurantMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Restaurant, MyRestaurantResponse>()
            .Map(d => d.AcceptingOrders, s => s.IsAcceptingOrders);

        config.NewConfig<Restaurant, RestaurantResponse>()
            .Map(d => d.AcceptingOrders, s => s.IsAcceptingOrders)
            .Map(d => d.OwnerEmail, s => s.Owner != null ? s.Owner.Email : string.Empty)
            .Map(d => d.Status, s => s.Status.ToString());

        config.NewConfig<Restaurant, PublicRestaurantResponse>()
            .Map(d => d.AcceptingOrders, s => s.IsAcceptingOrders);
    }
}
