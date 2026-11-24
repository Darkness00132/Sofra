namespace Sofra.Application.Features.Orders;

/// <summary>
/// Pricing rules applied at checkout. Bound from the "OrderFees" configuration section.
/// Placeholder values until the fees decision (PRD §10) is finalized.
/// </summary>
public class OrderFeesOptions
{
    public const string SectionName = "OrderFees";

    /// <summary>Tax applied on the order subtotal (0.14 = 14%).</summary>
    public decimal TaxRate { get; set; } = 0m;

    /// <summary>Flat fee charged on delivery orders; pickup orders are never charged.</summary>
    public decimal DeliveryFee { get; set; } = 0m;
}
