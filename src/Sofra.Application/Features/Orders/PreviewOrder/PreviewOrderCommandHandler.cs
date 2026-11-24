using MediatR;
using Microsoft.Extensions.Options;
using Sofra.Application.Common.Interfaces;
using Sofra.Application.Common.Results;
using Sofra.Domain.Enums;
using Sofra.Domain.Exceptions;

namespace Sofra.Application.Features.Orders.PreviewOrder;

public class PreviewOrderCommandHandler(
    ICurrentUser currentUser,
    ICartRepository carts,
    IRestaurantRepository restaurants,
    IOptions<OrderFeesOptions> feesOptions) : IRequestHandler<PreviewOrderCommand, Result<OrderPreviewResponse>>
{
    public async Task<Result<OrderPreviewResponse>> Handle(PreviewOrderCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.GetUserId();

        var cart = await carts.FindForUserAsync(userId, cancellationToken);
        if (cart is null || cart.IsEmpty || cart.RestaurantId is null)
            return Result<OrderPreviewResponse>.Failure(Error.BusinessRule(
                "CART_EMPTY",
                "Your cart is empty; add items before checking out."));

        var restaurant = await restaurants.FindAsync(cart.RestaurantId.Value, cancellationToken);
        if (restaurant is null || !restaurant.CanAcceptNewOrders)
            return Result<OrderPreviewResponse>.Failure(Error.BusinessRule(
                "RESTAURANT_NOT_ACCEPTING_ORDERS",
                "The restaurant is not accepting orders right now (BR-09)."));

        foreach (var cartItem in cart.CartItems)
        {
            if (!cartItem.FoodItem.IsAvailable)
                return Result<OrderPreviewResponse>.Failure(Error.BusinessRule(
                    "FOOD_ITEM_UNAVAILABLE",
                    $"'{cartItem.FoodItem.Name}' is no longer available (BR-05); remove it and try again."));
        }

        var subtotal = cart.CartItems.Sum(i => i.Quantity * i.FoodItem.Price);
        var pricing = OrderPricing.Calculate(subtotal, request.FulfilmentType, feesOptions.Value);

        return new OrderPreviewResponse(
            cart.RestaurantId.Value,
            restaurant.Name,
            subtotal,
            pricing.Tax,
            pricing.DeliveryFee,
            pricing.Total);
    }
}
