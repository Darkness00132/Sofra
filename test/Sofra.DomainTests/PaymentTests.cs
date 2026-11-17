using Sofra.Domain.Entities;
using Sofra.Domain.Enums;
using Sofra.Domain.Exceptions;

namespace Sofra.DomainTests;

/// <summary>Payment state rules — one transition per state, refunds only from completed (SRS FR-27/FR-28).</summary>
public class PaymentTests
{
    [Fact]
    public void A_payment_amount_must_be_greater_than_zero()
    {
        Assert.Throws<DomainException>(() =>
            Payment.Create(Guid.NewGuid(), amount: 0m, PaymentMethod.CashOnDelivery));
    }

    [Fact]
    public void A_new_payment_starts_pending()
    {
        var payment = Payment.Create(Guid.NewGuid(), 100m, PaymentMethod.CashOnDelivery);

        Assert.Equal(PaymentStatus.Pending, payment.Status);
    }

    [Fact]
    public void Completing_a_payment_records_the_paid_time_and_metadata()
    {
        var payment = Payment.Create(Guid.NewGuid(), 100m, PaymentMethod.OnlinePayment);

        payment.MarkCompleted("gateway-ref-1");

        Assert.Equal(PaymentStatus.Completed, payment.Status);
        Assert.NotNull(payment.PaidAt);
        Assert.Equal("gateway-ref-1", payment.Metadata);
    }

    [Fact]
    public void A_completed_payment_cannot_be_completed_again()
    {
        var payment = Payment.Create(Guid.NewGuid(), 100m, PaymentMethod.CashOnDelivery);
        payment.MarkCompleted();

        Assert.Throws<DomainException>(() => payment.MarkCompleted());
    }

    [Fact]
    public void A_completed_payment_cannot_be_cancelled()
    {
        var payment = Payment.Create(Guid.NewGuid(), 100m, PaymentMethod.CashOnDelivery);
        payment.MarkCompleted();

        Assert.Throws<DomainException>(() => payment.Cancel());
    }

    [Fact]
    public void Only_a_completed_payment_can_be_refunded()
    {
        var payment = Payment.Create(Guid.NewGuid(), 100m, PaymentMethod.OnlinePayment);

        Assert.Throws<DomainException>(() => payment.Refund());
    }

    [Fact]
    public void Refunding_a_completed_payment_marks_it_refunded()
    {
        var payment = Payment.Create(Guid.NewGuid(), 100m, PaymentMethod.OnlinePayment);
        payment.MarkCompleted();

        payment.Refund();

        Assert.Equal(PaymentStatus.Refunded, payment.Status);
    }
}
