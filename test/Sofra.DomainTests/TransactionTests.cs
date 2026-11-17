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
        Assert.Throws<DomainException>(() => Transaction.Create(TransactionType.Debit, amount: 0m));
    }

    [Fact]
    public void A_new_transaction_is_completed_by_default()
    {
        var transaction = Transaction.Create(TransactionType.Credit, 100m);

        Assert.Equal(TransactionStatus.Completed, transaction.Status);
    }

    [Fact]
    public void A_reversed_transaction_cannot_be_marked_failed()
    {
        var transaction = Transaction.Create(TransactionType.Debit, 100m);
        transaction.Reverse();

        Assert.Throws<DomainException>(() => transaction.MarkFailed());
    }

    [Fact]
    public void A_failed_transaction_cannot_be_marked_failed_again()
    {
        var transaction = Transaction.Create(TransactionType.Debit, 100m);
        transaction.MarkFailed();

        Assert.Throws<DomainException>(() => transaction.MarkFailed());
    }

    [Fact]
    public void Only_a_completed_transaction_can_be_reversed()
    {
        var transaction = Transaction.Create(TransactionType.Debit, 100m);
        transaction.MarkFailed();

        Assert.Throws<DomainException>(() => transaction.Reverse());
    }
}
