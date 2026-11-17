using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sofra.Domain.Entities;

namespace Sofra.Infrastructure.Persistent.Configurations;

internal sealed class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        // BR-04: the price snapshot must never be lost — deleting a menu item cannot take
        // historical order rows with it
        builder.Property(i => i.Price).HasPrecision(18, 2);

        builder.HasOne(i => i.Order)
            .WithMany(o => o.OrderItems)
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(i => i.FoodItem)
            .WithMany(f => f.OrderItems)
            .HasForeignKey(i => i.FoodItemId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
