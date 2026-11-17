using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sofra.Domain.Entities;

namespace Sofra.Infrastructure.Persistent.Configurations;

internal sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.Property(o => o.DeliveryAddress).HasMaxLength(500);

        builder.Property(o => o.Subtotal).HasPrecision(18, 2);
        builder.Property(o => o.Tax).HasPrecision(18, 2);
        builder.Property(o => o.DeliveryFee).HasPrecision(18, 2);
        builder.Property(o => o.Discount).HasPrecision(18, 2);
        builder.Property(o => o.Total).HasPrecision(18, 2);

        builder.HasOne(o => o.Customer)
            .WithMany()
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.Restaurant)
            .WithMany(r => r.Orders)
            .HasForeignKey(o => o.RestaurantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
