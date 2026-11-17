using Sofra.Domain.Entities;
using Sofra.Domain.Enums;
using Sofra.Domain.Exceptions;

namespace Sofra.DomainTests;

/// <summary>
/// Order lifecycle guards, totals integrity and delivery tracking (SRS FR-25..FR-38, BR-06..BR-08, BR-11).
/// </summary>
public class OrderTests
{
    private static readonly Guid CustomerId = Guid.NewGuid();
    private static readonly Guid RestaurantId = Guid.NewGuid();
    private static readonly Guid StaffId = Guid.NewGuid();

    [Fact]
    public void A_new_order_starts_pending_with_zero_totals()
    {
        var order = PickupOrder();

        Assert.Equal(OrderStatus.Pending, order.Status);
        Assert.Equal(0m, order.Total);
    }

    [Fact]
    public void A_delivery_order_requires_a_delivery_address()
    {
        Assert.Throws<DomainException>(() => Order.Create(
            CustomerId, RestaurantId, FulfilmentType.Delivery, PaymentMethod.CashOnDelivery, deliveryAddress: null));
    }

    [Fact]
    public void A_delivery_order_starts_with_pending_delivery_tracking()
    {
        var order = DeliveryOrder();

        Assert.Equal(DeliveryStatus.Pending, order.DeliveryStatus);
    }

    [Fact]
    public void A_pickup_order_does_not_track_delivery()
    {
        var order = PickupOrder();

        Assert.Equal(DeliveryStatus.NotApplicable, order.DeliveryStatus);
    }

    [Fact]
    public void Items_can_only_be_added_while_the_order_is_pending()
    {
        var order = PickupOrder();
        order.Accept(StaffId);

        Assert.Throws<DomainException>(() => order.AddItem(Guid.NewGuid(), quantity: 1, unitPrice: 50m));
    }

    [Fact]
    public void An_order_item_quantity_must_be_greater_than_zero()
    {
        var order = PickupOrder();

        Assert.Throws<DomainException>(() => order.AddItem(Guid.NewGuid(), quantity: 0, unitPrice: 50m));
    }

    [Fact]
    public void An_order_item_price_cannot_be_negative()
    {
        var order = PickupOrder();

        Assert.Throws<DomainException>(() => order.AddItem(Guid.NewGuid(), quantity: 1, unitPrice: -1m));
    }

    [Fact]
    public void An_order_item_can_be_removed_while_the_order_is_pending()
    {
        var order = PickupOrder();
        var item = order.AddItem(Guid.NewGuid(), quantity: 1, unitPrice: 50m);

        order.RemoveItem(item.Id);

        Assert.Empty(order.OrderItems);
    }

    [Fact]
    public void Totals_cannot_be_set_on_an_order_without_items()
    {
        var order = PickupOrder();

        Assert.Throws<DomainException>(() => order.SetTotals(tax: 0m, deliveryFee: 0m, discount: 0m));
    }

    [Fact]
    public void Totals_cannot_contain_negative_values()
    {
        var order = PickupOrder();
        order.AddItem(Guid.NewGuid(), quantity: 1, unitPrice: 50m);

        Assert.Throws<DomainException>(() => order.SetTotals(tax: -1m, deliveryFee: 0m, discount: 0m));
    }

    [Fact]
    public void The_discount_cannot_exceed_the_order_amount()
    {
        var order = PickupOrder();
        order.AddItem(Guid.NewGuid(), quantity: 2, unitPrice: 50m);   // subtotal 100

        Assert.Throws<DomainException>(() => order.SetTotals(tax: 0m, deliveryFee: 0m, discount: 150m));
    }

    [Fact]
    public void The_order_total_is_subtotal_plus_tax_and_delivery_fee_minus_discount()
    {
        var order = PickupOrder();
        order.AddItem(Guid.NewGuid(), quantity: 2, unitPrice: 50m);   // subtotal 100

        order.SetTotals(tax: 5m, deliveryFee: 10m, discount: 10m);

        Assert.Equal(100m, order.Subtotal);
        Assert.Equal(105m, order.Total);
    }

    [Fact]
    public void An_order_can_only_be_accepted_while_pending()
    {
        var order = PickupOrder();
        order.Accept(StaffId);

        Assert.Equal(OrderStatus.Accepted, order.Status);
        Assert.Throws<DomainException>(() => order.Accept(StaffId));
    }

    [Fact]
    public void An_order_can_only_start_preparing_from_accepted()
    {
        var order = PickupOrder();

        Assert.Throws<DomainException>(() => order.StartPreparing(StaffId));
    }

    [Fact]
    public void An_order_can_only_be_marked_ready_from_preparing()
    {
        var order = PickupOrder();
        order.Accept(StaffId);

        Assert.Throws<DomainException>(() => order.MarkReady(StaffId));
    }

    [Fact]
    public void An_order_can_only_be_completed_from_ready()
    {
        var order = PickupOrder();
        order.Accept(StaffId);

        Assert.Throws<DomainException>(() => order.Complete(StaffId));
    }

    [Fact]
    public void An_order_can_only_be_rejected_while_pending()
    {
        var order = PickupOrder();
        order.Accept(StaffId);

        Assert.Throws<DomainException>(() => order.Reject("Out of stock", StaffId));
    }

    [Fact]
    public void A_pending_order_can_be_cancelled()
    {
        var order = PickupOrder();

        order.Cancel(OrderCancellationReason.CustomerRequested, note: null, changedBy: CustomerId);

        Assert.Equal(OrderStatus.Cancelled, order.Status);
    }

    [Fact]
    public void A_completed_order_cannot_be_cancelled()
    {
        var order = PickupOrder();
        order.Accept(StaffId);
        order.StartPreparing(StaffId);
        order.MarkReady(StaffId);
        order.Complete(StaffId);

        Assert.Throws<DomainException>(() =>
            order.Cancel(OrderCancellationReason.CustomerRequested, note: null, changedBy: CustomerId));
    }

    [Fact]
    public void Completing_an_order_stamps_the_completion_time()
    {
        var order = PickupOrder();
        order.Accept(StaffId);
        order.StartPreparing(StaffId);
        order.MarkReady(StaffId);

        order.Complete(StaffId);

        Assert.Equal(OrderStatus.Completed, order.Status);
        Assert.NotNull(order.CompletedAt);
    }

    [Fact]
    public void Marking_a_delivery_order_ready_collects_it()
    {
        var order = DeliveryOrder();
        order.Accept(StaffId);
        order.StartPreparing(StaffId);

        order.MarkReady(StaffId);

        Assert.Equal(OrderStatus.Ready, order.Status);
        Assert.Equal(DeliveryStatus.Collected, order.DeliveryStatus);
    }

    [Fact]
    public void Completing_a_delivery_order_marks_it_delivered()
    {
        var order = DeliveryOrder();
        order.Accept(StaffId);
        order.StartPreparing(StaffId);
        order.MarkReady(StaffId);

        order.Complete(StaffId);

        Assert.Equal(DeliveryStatus.Delivered, order.DeliveryStatus);
    }

    [Fact]
    public void Completing_a_pickup_order_leaves_delivery_status_not_applicable()
    {
        var order = PickupOrder();
        order.Accept(StaffId);
        order.StartPreparing(StaffId);
        order.MarkReady(StaffId);

        order.Complete(StaffId);

        Assert.Equal(DeliveryStatus.NotApplicable, order.DeliveryStatus);
    }

    [Fact]
    public void Every_status_change_is_recorded_in_the_order_history()
    {
        var order = PickupOrder();

        order.Accept(StaffId);
        order.StartPreparing(StaffId);

        Assert.Equal(2, order.StatusHistory.Count);
        Assert.Equal(OrderStatus.Accepted, order.StatusHistory.ElementAt(0).Status);
        Assert.Equal(OrderStatus.Preparing, order.StatusHistory.ElementAt(1).Status);
        Assert.All(order.StatusHistory, h => Assert.Equal(StaffId, h.ChangedByUserId));
    }

    private static Order PickupOrder() => Order.Create(
        CustomerId, RestaurantId, FulfilmentType.Pickup, PaymentMethod.CashOnDelivery);

    private static Order DeliveryOrder() => Order.Create(
        CustomerId, RestaurantId, FulfilmentType.Delivery, PaymentMethod.CashOnDelivery,
        deliveryAddress: "12 Nile St, Cairo");
}
