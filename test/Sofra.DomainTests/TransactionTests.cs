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
        // Act
        Action create = () => Debit(amount: 0m);

        // Assert
        create.Should().Throw<DomainException>();
    }

    [Fact]
    public void A_new_transaction_is_completed_by_default()
    {
        // Act
        var transaction = Credit();

        // Assert
        transaction.Status.Should().Be(TransactionStatus.Completed);
    }

    [Fact]
    public void A_reversed_transaction_cannot_be_marked_failed()
    {
        // Arrange
        var transaction = Debit();
        transaction.Reverse();

        // Act
        Action markFailed = () => transaction.MarkFailed();

        // Assert
        markFailed.Should().Throw<DomainException>();
    }

    [Fact]
    public void A_failed_transaction_cannot_be_marked_failed_again()
    {
        // Arrange
        var transaction = Debit();
        transaction.MarkFailed();

        // Act
        Action markFailedAgain = () => transaction.MarkFailed();

        // Assert
        markFailedAgain.Should().Throw<DomainException>();
    }

    [Fact]
    public void Only_a_completed_transaction_can_be_reversed()
    {
        // Arrange
        var transaction = Debit();
        transaction.MarkFailed();

        // Act
        Action reverse = () => transaction.Reverse();

        // Assert
        reverse.Should().Throw<DomainException>();
    }

    private static Transaction Debit(decimal amount = 100m) =>
        Transaction.Create(TransactionType.Debit, amount);

    private static Transaction Credit(decimal amount = 100m) =>
        Transaction.Create(TransactionType.Credit, amount);
}
