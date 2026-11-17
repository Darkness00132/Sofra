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
        // Act
        var order = PickupOrder();

        // Assert
        order.Status.Should().Be(OrderStatus.Pending);
        order.Total.Should().Be(0m);
    }

    [Fact]
    public void A_delivery_order_requires_a_delivery_address()
    {
        // Act
        Action create = () => DeliveryOrder(deliveryAddress: null);

        // Assert
        create.Should().Throw<DomainException>();
    }

    [Fact]
    public void A_delivery_order_starts_with_pending_delivery_tracking()
    {
        // Act
        var order = DeliveryOrder();

        // Assert
        order.DeliveryStatus.Should().Be(DeliveryStatus.Pending);
    }

    [Fact]
    public void A_pickup_order_does_not_track_delivery()
    {
        // Act
        var order = PickupOrder();

        // Assert
        order.DeliveryStatus.Should().Be(DeliveryStatus.NotApplicable);
    }

    [Fact]
    public void Items_can_only_be_added_while_the_order_is_pending()
    {
        // Arrange
        var order = PickupOrder();
        order.Accept(StaffId);

        // Act
        Action addItem = () => AddBurger(order);

        // Assert
        addItem.Should().Throw<DomainException>();
    }

    [Fact]
    public void An_order_item_quantity_must_be_greater_than_zero()
    {
        // Arrange
        var order = PickupOrder();

        // Act
        Action addItem = () => AddBurger(order, quantity: 0);

        // Assert
        addItem.Should().Throw<DomainException>();
    }

    [Fact]
    public void An_order_item_price_cannot_be_negative()
    {
        // Arrange
        var order = PickupOrder();

        // Act
        Action addItem = () => AddBurger(order, unitPrice: -1m);

        // Assert
        addItem.Should().Throw<DomainException>();
    }

    [Fact]
    public void An_order_item_can_be_removed_while_the_order_is_pending()
    {
        // Arrange
        var order = PickupOrder();
        var item = AddBurger(order);

        // Act
        order.RemoveItem(item.Id);

        // Assert
        order.OrderItems.Should().BeEmpty();
    }

    [Fact]
    public void Totals_cannot_be_set_on_an_order_without_items()
    {
        // Arrange
        var order = PickupOrder();

        // Act
        Action setTotals = () => order.SetTotals(tax: 0m, deliveryFee: 0m, discount: 0m);

        // Assert
        setTotals.Should().Throw<DomainException>();
    }

    [Fact]
    public void Totals_cannot_contain_negative_values()
    {
        // Arrange
        var order = PickupOrder();
        AddBurger(order);

        // Act
        Action setTotals = () => order.SetTotals(tax: -1m, deliveryFee: 0m, discount: 0m);

        // Assert
        setTotals.Should().Throw<DomainException>();
    }

    [Fact]
    public void The_discount_cannot_exceed_the_order_amount()
    {
        // Arrange
        var order = PickupOrder();
        AddBurger(order, quantity: 2);

        // Act
        Action setTotals = () => order.SetTotals(tax: 0m, deliveryFee: 0m, discount: 150m);

        // Assert
        setTotals.Should().Throw<DomainException>();
    }

    [Fact]
    public void The_order_total_is_subtotal_plus_tax_and_delivery_fee_minus_discount()
    {
        // Arrange
        var order = PickupOrder();
        AddBurger(order, quantity: 2);

        // Act
        order.SetTotals(tax: 5m, deliveryFee: 10m, discount: 10m);

        // Assert
        order.Subtotal.Should().Be(100m);
        order.Total.Should().Be(105m);
    }

    [Fact]
    public void An_order_can_only_be_accepted_while_pending()
    {
        // Arrange
        var order = PickupOrder();

        // Act
        order.Accept(StaffId);

        // Assert
        order.Status.Should().Be(OrderStatus.Accepted);

        Action acceptAgain = () => order.Accept(StaffId);
        acceptAgain.Should().Throw<DomainException>();
    }

    [Fact]
    public void An_order_can_only_start_preparing_from_accepted()
    {
        // Arrange
        var order = PickupOrder();

        // Act
        Action startPreparing = () => order.StartPreparing(StaffId);

        // Assert
        startPreparing.Should().Throw<DomainException>();
    }

    [Fact]
    public void An_order_can_only_be_marked_ready_from_preparing()
    {
        // Arrange
        var order = PickupOrder();
        order.Accept(StaffId);

        // Act
        Action markReady = () => order.MarkReady(StaffId);

        // Assert
        markReady.Should().Throw<DomainException>();
    }

    [Fact]
    public void An_order_can_only_be_completed_from_ready()
    {
        // Arrange
        var order = PickupOrder();
        order.Accept(StaffId);

        // Act
        Action complete = () => order.Complete(StaffId);

        // Assert
        complete.Should().Throw<DomainException>();
    }

    [Fact]
    public void An_order_can_only_be_rejected_while_pending()
    {
        // Arrange
        var order = PickupOrder();
        order.Accept(StaffId);

        // Act
        Action reject = () => order.Reject("Out of stock", StaffId);

        // Assert
        reject.Should().Throw<DomainException>();
    }

    [Fact]
    public void A_pending_order_can_be_cancelled()
    {
        // Arrange
        var order = PickupOrder();

        // Act
        order.Cancel(OrderCancellationReason.CustomerRequested, note: null, changedBy: CustomerId);

        // Assert
        order.Status.Should().Be(OrderStatus.Cancelled);
    }

    [Fact]
    public void A_completed_order_cannot_be_cancelled()
    {
        // Arrange
        var order = CompletedOrder();

        // Act
        Action cancel = () =>
            order.Cancel(OrderCancellationReason.CustomerRequested, note: null, changedBy: CustomerId);

        // Assert
        cancel.Should().Throw<DomainException>();
    }

    [Fact]
    public void Completing_an_order_stamps_the_completion_time()
    {
        // Arrange
        var order = ReadyOrder();

        // Act
        order.Complete(StaffId);

        // Assert
        order.Status.Should().Be(OrderStatus.Completed);
        order.CompletedAt.Should().NotBeNull();
    }

    [Fact]
    public void Marking_a_delivery_order_ready_collects_it()
    {
        // Arrange
        var order = DeliveryOrder();

        // Act
        TakeToReady(order);

        // Assert
        order.Status.Should().Be(OrderStatus.Ready);
        order.DeliveryStatus.Should().Be(DeliveryStatus.Collected);
    }

    [Fact]
    public void Completing_a_delivery_order_marks_it_delivered()
    {
        // Arrange
        var order = DeliveryOrder();
        TakeToReady(order);

        // Act
        order.Complete(StaffId);

        // Assert
        order.DeliveryStatus.Should().Be(DeliveryStatus.Delivered);
    }

    [Fact]
    public void Completing_a_pickup_order_leaves_delivery_status_not_applicable()
    {
        // Arrange
        var order = ReadyOrder();

        // Act
        order.Complete(StaffId);

        // Assert
        order.DeliveryStatus.Should().Be(DeliveryStatus.NotApplicable);
    }

    [Fact]
    public void Every_status_change_is_recorded_in_the_order_history()
    {
        // Arrange
        var order = PickupOrder();

        // Act
        order.Accept(StaffId);
        order.StartPreparing(StaffId);

        // Assert
        var history = order.StatusHistory.ToList();
        history.Should().HaveCount(2);
        history[0].Status.Should().Be(OrderStatus.Accepted);
        history[1].Status.Should().Be(OrderStatus.Preparing);

        foreach (var entry in history)
            entry.ChangedByUserId.Should().Be(StaffId);
    }

    private static OrderItem AddBurger(Order order, int quantity = 1, decimal unitPrice = 50m) =>
        order.AddItem(Guid.NewGuid(), quantity, unitPrice);

    private static void TakeToReady(Order order)
    {
        order.Accept(StaffId);
        order.StartPreparing(StaffId);
        order.MarkReady(StaffId);
    }

    private static Order PickupOrder() => Order.Create(
        CustomerId, RestaurantId, FulfilmentType.Pickup, PaymentMethod.CashOnDelivery);

    private static Order DeliveryOrder(string? deliveryAddress = "12 Nile St, Cairo") => Order.Create(
        CustomerId, RestaurantId, FulfilmentType.Delivery, PaymentMethod.CashOnDelivery, deliveryAddress);

    private static Order ReadyOrder()
    {
        var order = PickupOrder();
        TakeToReady(order);

        return order;
    }

    private static Order CompletedOrder()
    {
        var order = ReadyOrder();
        order.Complete(StaffId);

        return order;
    }
}
