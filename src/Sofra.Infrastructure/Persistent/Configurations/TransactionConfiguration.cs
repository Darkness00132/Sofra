using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sofra.Domain.Entities;

namespace Sofra.Infrastructure.Persistent.Configurations;

internal sealed class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.Property(t => t.Amount).HasPrecision(18, 2);
        builder.Property(t => t.Description).HasMaxLength(500);
        builder.Property(t => t.ReferenceNumber).HasMaxLength(100);

        // the ledger stays denormalized — all three FKs are optional and null out gracefully
        builder.HasOne(t => t.Order)
            .WithMany(o => o.Transactions)
            .HasForeignKey(t => t.OrderId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(t => t.Restaurant)
            .WithMany()
            .HasForeignKey(t => t.RestaurantId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
