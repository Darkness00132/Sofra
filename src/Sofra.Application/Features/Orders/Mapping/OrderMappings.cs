using Mapster;
using Sofra.Application.Features.Orders.Dtos;
using Sofra.Domain.Entities;

namespace Sofra.Application.Features.Orders.Mapping;

/// <summary>
/// Order → response mappings. Nothing here maps from same-name members alone: the item's
/// display name and LineTotal come from OrderItem.FoodItem / the computed quantity*price,
/// PaymentStatus from the first Payment, enums are stored as strings, and RestaurantName
/// from the navigation. The registered config drives both the SQL <c>ProjectTo</c>
/// (server-side, in the repository) and in-memory <c>Adapt</c> (mutation handlers after
/// SaveChanges) — one source of truth.
/// </summary>
public class OrderMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<OrderItem, OrderItemResponse>()
            .Map(d => d.FoodItemName, s => s.FoodItem != null ? s.FoodItem.Name : string.Empty)
            .Map(d => d.UnitPrice, s => s.Price)
            .Map(d => d.LineTotal, s => s.Quantity * s.Price);

        config.NewConfig<OrderStatusHistory, OrderStatusHistoryResponse>()
            .Map(d => d.Status, s => s.Status.ToString())
            .Map(d => d.ChangedByUserId, s => s.ChangedByUserId)
            .Map(d => d.Reason, s => s.Reason)
            .Map(d => d.ChangedAt, s => s.ChangedAt);

        config.NewConfig<Order, OrderDetailsResponse>()
            .Map(d => d.RestaurantName, s => s.Restaurant != null ? s.Restaurant.Name : string.Empty)
            .Map(d => d.Status, s => s.Status.ToString())
            .Map(d => d.FulfilmentType, s => s.FulfilmentType.ToString())
            .Map(d => d.DeliveryStatus, s => s.DeliveryStatus.ToString())
            .Map(d => d.PaymentMethod, s => s.PaymentMethod.ToString())
            .Map(d => d.PaymentStatus, s => s.Payments.Any() ? s.Payments.First().Status.ToString() : null)
            .Map(d => d.Items, s => s.OrderItems)
            .Map(d => d.StatusHistory, s => s.StatusHistory);

        config.NewConfig<Order, OrderSummaryResponse>()
            .Map(d => d.RestaurantName, s => s.Restaurant != null ? s.Restaurant.Name : string.Empty)
            .Map(d => d.Status, s => s.Status.ToString())
            .Map(d => d.FulfilmentType, s => s.FulfilmentType.ToString())
            .Map(d => d.DeliveryStatus, s => s.DeliveryStatus.ToString())
            .Map(d => d.PaymentMethod, s => s.PaymentMethod.ToString());
    }
}
