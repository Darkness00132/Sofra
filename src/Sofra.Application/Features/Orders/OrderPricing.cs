using Sofra.Domain.Enums;

namespace Sofra.Application.Features.Orders;

/// <summary>Computed checkout charges.</summary>
/// <param name="Tax">Tax on the subtotal.</param>
/// <param name="DeliveryFee">Flat fee charged on delivery orders; zero for pickup.</param>
/// <param name="Total">Subtotal + tax + delivery fee.</param>
public record OrderCharges(decimal Tax, decimal DeliveryFee, decimal Total);

/// <summary>Computes checkout pricing. Shared by order placement and order preview (FR-27).</summary>
public static class OrderPricing
{
    public static OrderCharges Calculate(decimal subtotal, FulfilmentType fulfilmentType, OrderFeesOptions fees)
    {
        var deliveryFee = fulfilmentType == FulfilmentType.Delivery ? fees.DeliveryFee : 0m;
        var tax = Math.Round(subtotal * fees.TaxRate, 2);

        return new OrderCharges(tax, deliveryFee, subtotal + tax + deliveryFee);
    }
}
