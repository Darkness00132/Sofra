using Sofra.Domain.Entities;
using Sofra.Domain.Enums;
using Sofra.Domain.Exceptions;

namespace Sofra.DomainTests;

/// <summary>Ledger rules — terminal transactions are frozen (SRS NFR-07).</summary>
public class TransactionTests
{
    [Fact]
    public void A_transaction_amount_must_be_greater_than_zero()
    {
        FluentActions.Invoking(() => LedgerEntry(amount: 0m))
            .Should().Throw<DomainException>();
    }

    [Fact]
    public void A_new_transaction_is_completed_by_default()
    {
        var transaction = LedgerEntry(type: TransactionType.Credit);

        transaction.Status.Should().Be(TransactionStatus.Completed);
    }

    [Fact]
    public void A_reversed_transaction_cannot_be_marked_failed()
    {
        var transaction = LedgerEntry();
        transaction.Reverse();

        FluentActions.Invoking(() => transaction.MarkFailed())
            .Should().Throw<DomainException>();
    }

    [Fact]
    public void A_failed_transaction_cannot_be_marked_failed_again()
    {
        var transaction = LedgerEntry();
        transaction.MarkFailed();

        FluentActions.Invoking(() => transaction.MarkFailed())
            .Should().Throw<DomainException>();
    }

    [Fact]
    public void Only_a_completed_transaction_can_be_reversed()
    {
        var transaction = LedgerEntry();
        transaction.MarkFailed();

        FluentActions.Invoking(() => transaction.Reverse())
            .Should().Throw<DomainException>();
    }

    private static Transaction LedgerEntry(decimal amount = 100m, TransactionType type = TransactionType.Debit) =>
        Transaction.Create(type, amount);
}
