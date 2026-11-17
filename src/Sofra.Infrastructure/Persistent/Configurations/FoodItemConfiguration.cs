using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sofra.Domain.Entities;

namespace Sofra.Infrastructure.Persistent.Configurations;

internal sealed class FoodItemConfiguration : IEntityTypeConfiguration<FoodItem>
{
    public void Configure(EntityTypeBuilder<FoodItem> builder)
    {
        builder.Property(i => i.Name).HasMaxLength(200).IsRequired();
        builder.Property(i => i.Description).HasMaxLength(1000).IsRequired();
        builder.Property(i => i.Price).HasPrecision(18, 2);

        builder.HasOne(i => i.FoodType)
            .WithMany(t => t.FoodItems)
            .HasForeignKey(i => i.FoodTypeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
