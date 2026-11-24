using Mapster;
using MediatR;
using Microsoft.Extensions.Options;
using Sofra.Application.Common.Interfaces;
using Sofra.Application.Common.Results;
using Sofra.Application.Features.Orders.Dtos;
using Sofra.Domain.Entities;
using Sofra.Domain.Enums;
using Sofra.Domain.Exceptions;

namespace Sofra.Application.Features.Orders.PlaceOrder;

public class PlaceOrderCommandHandler(
    ICurrentUser currentUser,
    ICartRepository carts,
    IRestaurantRepository restaurants,
    IOrderRepository orders,
    IPaymentRepository payments,
    IUnitOfWork unitOfWork,
    IOptions<OrderFeesOptions> feesOptions) : IRequestHandler<PlaceOrderCommand, Result<PlaceOrderResponse>>
{
    public async Task<Result<PlaceOrderResponse>> Handle(PlaceOrderCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.GetUserId();

        var cart = await carts.FindForUserAsync(userId, cancellationToken);
        if (cart is null || cart.IsEmpty || cart.RestaurantId is null)
            return Result<PlaceOrderResponse>.Failure(Error.BusinessRule(
                "CART_EMPTY",
                "Your cart is empty; add items before checking out."));

        var restaurant = await restaurants.FindAsync(cart.RestaurantId.Value, cancellationToken);
        if (restaurant is null || !restaurant.CanAcceptNewOrders)
            return Result<PlaceOrderResponse>.Failure(Error.BusinessRule(
                "RESTAURANT_NOT_ACCEPTING_ORDERS",
                "The restaurant is not accepting orders right now (BR-09)."));

        var order = Order.Create(userId, cart.RestaurantId.Value, request.FulfilmentType, request.PaymentMethod, request.DeliveryAddress);

        foreach (var cartItem in cart.CartItems)
        {
            var foodItem = cartItem.FoodItem;

            if (!foodItem.IsAvailable)
                return Result<PlaceOrderResponse>.Failure(Error.BusinessRule(
                    "FOOD_ITEM_UNAVAILABLE",
                    $"'{foodItem.Name}' is no longer available (BR-05); remove it and try again."));

            order.AddItem(foodItem.Id, cartItem.Quantity, foodItem.Price);   // BR-04: price snapshot
        }

        var subtotal = order.OrderItems.Sum(i => i.LineTotal);
        var pricing = OrderPricing.Calculate(subtotal, request.FulfilmentType, feesOptions.Value);

        try
        {
            order.SetTotals(pricing.Tax, pricing.DeliveryFee, discount: 0m);
        }
        catch (DomainException ex)
        {
            return Result<PlaceOrderResponse>.Failure(Error.BusinessRule("ORDER_TOTALS_INVALID", ex.Message));
        }

        await orders.AddAsync(order, cancellationToken);
        await payments.AddAsync(Payment.Create(order.Id, order.Total, request.PaymentMethod), cancellationToken);
        cart.Clear();

        // One context, one save: the order, its payment and the cleared cart commit together.
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return order.Adapt<PlaceOrderResponse>();
    }
}
