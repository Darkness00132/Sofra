using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sofra.Domain.Entities;

namespace Sofra.Infrastructure.Persistent.Configurations;

internal sealed class RestaurantConfiguration : IEntityTypeConfiguration<Restaurant>
{
    public void Configure(EntityTypeBuilder<Restaurant> builder)
    {
        builder.Property(r => r.Name).HasMaxLength(200).IsRequired();
        builder.Property(r => r.Address).HasMaxLength(500).IsRequired();
        builder.Property(r => r.Phone).HasMaxLength(30).IsRequired();
        builder.Property(r => r.Description).HasMaxLength(1000).IsRequired();

        builder.HasOne(r => r.Owner)
            .WithMany()
            .HasForeignKey(r => r.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
