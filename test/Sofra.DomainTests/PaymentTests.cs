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
        FluentActions.Invoking(() => PaymentOf(amount: 0m))
            .Should().Throw<DomainException>();
    }

    [Fact]
    public void A_new_payment_starts_pending()
    {
        var payment = PaymentOf();

        payment.Status.Should().Be(PaymentStatus.Pending);
    }

    [Fact]
    public void Completing_a_payment_records_the_paid_time_and_metadata()
    {
        var payment = PaymentOf(method: PaymentMethod.OnlinePayment);

        payment.MarkCompleted("gateway-ref-1");

        payment.Status.Should().Be(PaymentStatus.Completed);
        payment.PaidAt.Should().NotBeNull();
        payment.Metadata.Should().Be("gateway-ref-1");
    }

    [Fact]
    public void A_completed_payment_cannot_be_completed_again()
    {
        var payment = CompletedPayment();

        FluentActions.Invoking(() => payment.MarkCompleted())
            .Should().Throw<DomainException>();
    }

    [Fact]
    public void A_completed_payment_cannot_be_cancelled()
    {
        var payment = CompletedPayment();

        FluentActions.Invoking(() => payment.Cancel())
            .Should().Throw<DomainException>();
    }

    [Fact]
    public void Only_a_completed_payment_can_be_refunded()
    {
        var payment = PaymentOf(method: PaymentMethod.OnlinePayment);

        FluentActions.Invoking(() => payment.Refund())
            .Should().Throw<DomainException>();
    }

    [Fact]
    public void Refunding_a_completed_payment_marks_it_refunded()
    {
        var payment = CompletedPayment(method: PaymentMethod.OnlinePayment);

        payment.Refund();

        payment.Status.Should().Be(PaymentStatus.Refunded);
    }

    private static Payment PaymentOf(decimal amount = 100m, PaymentMethod method = PaymentMethod.CashOnDelivery) =>
        Payment.Create(Guid.NewGuid(), amount, method);

    private static Payment CompletedPayment(PaymentMethod method = PaymentMethod.CashOnDelivery)
    {
        var payment = PaymentOf(method: method);
        payment.MarkCompleted();

        return payment;
    }
}
