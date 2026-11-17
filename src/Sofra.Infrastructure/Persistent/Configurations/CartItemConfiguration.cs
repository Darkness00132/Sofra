using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sofra.Domain.Entities;

namespace Sofra.Infrastructure.Persistent.Configurations;

internal sealed class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.HasOne(i => i.Cart)
            .WithMany(c => c.CartItems)
            .HasForeignKey(i => i.CartId)
            .OnDelete(DeleteBehavior.Cascade);

        // keep the cart usable even if the item is later removed from the menu
        builder.HasOne(i => i.FoodItem)
            .WithMany(f => f.CartItems)
            .HasForeignKey(i => i.FoodItemId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
