using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sofra.Domain.Entities;

namespace Sofra.Infrastructure.Persistent.Configurations;

internal sealed class FoodTypeConfiguration : IEntityTypeConfiguration<FoodType>
{
    public void Configure(EntityTypeBuilder<FoodType> builder)
    {
        builder.Property(t => t.Name).HasMaxLength(100).IsRequired();
        builder.Property(t => t.Description).HasMaxLength(1000);

        builder.HasOne(t => t.Restaurant)
            .WithMany(r => r.FoodTypes)
            .HasForeignKey(t => t.RestaurantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(t => t.FoodCategory)
            .WithMany(c => c.FoodTypes)
            .HasForeignKey(t => t.FoodCategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
