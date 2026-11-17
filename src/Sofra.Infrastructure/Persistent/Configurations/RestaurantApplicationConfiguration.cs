using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sofra.Domain.Entities;

namespace Sofra.Infrastructure.Persistent.Configurations;

internal sealed class RestaurantApplicationConfiguration : IEntityTypeConfiguration<RestaurantApplication>
{
    public void Configure(EntityTypeBuilder<RestaurantApplication> builder)
    {
        builder.Property(a => a.RestaurantName).HasMaxLength(200).IsRequired();
        builder.Property(a => a.Address).HasMaxLength(500).IsRequired();
        builder.Property(a => a.Phone).HasMaxLength(30).IsRequired();
        builder.Property(a => a.Description).HasMaxLength(1000).IsRequired();
        builder.Property(a => a.Documents).HasMaxLength(2000);
        builder.Property(a => a.RejectionReason).HasMaxLength(1000);

        builder.HasOne(a => a.Applicant)
            .WithMany()
            .HasForeignKey(a => a.ApplicantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.ReviewedByUser)
            .WithMany()
            .HasForeignKey(a => a.ReviewedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // 0..1 — linked only after the application is approved (Restaurant.LinkApprovedApplication)
        builder.HasOne(a => a.Restaurant)
            .WithMany(r => r.Applications)
            .HasForeignKey(a => a.RestaurantId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
