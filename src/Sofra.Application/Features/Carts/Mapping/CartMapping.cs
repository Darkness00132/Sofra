using Mapster;
using Sofra.Application.Features.Carts.Dtos;
using Sofra.Domain.Entities;

namespace Sofra.Application.Features.Carts.Mapping;

/// <summary>
/// CartResponse/ItemResponse can't be mapped from same-name members alone: food name and price
/// come from the FoodItem navigation, and LineTotal/Subtotal are computed. CartRepository uses
/// this config with Mapster's ProjectToType, so the expressions become SQL and only the needed
/// columns are read — the response never maps a tracked entity in memory.
/// </summary>
public class CartMapping : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<FoodItem, FoodItemForCartResponse>()
            .Map(d => d.RestaurantId, s => s.FoodType.RestaurantId);

        config.NewConfig<CartItem, CartItemResponse>()
            .Map(d => d.FoodItemName, s => s.FoodItem.Name)
            .Map(d => d.UnitPrice, s => s.FoodItem.Price)
            .Map(d => d.LineTotal, s => s.Quantity * s.FoodItem.Price);

        config.NewConfig<Cart, CartResponse>()
            .Map(d => d.RestaurantName, s => s.Restaurant != null ? s.Restaurant.Name : null)
            .Map(d => d.Items, s => s.CartItems)
            .Map(d => d.Subtotal, s => s.CartItems.Sum(i => i.Quantity * i.FoodItem.Price));
    }
}
