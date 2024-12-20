using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UtilityBills.Aggregates.UtilityPaymentPlatformAggregate;

namespace UtilityBills.Infrastructure.EntityConfigurations;

public class UtilityPaymentPlatformConfiguration : IEntityTypeConfiguration<UtilityPaymentPlatform>
{
    public void Configure(EntityTypeBuilder<UtilityPaymentPlatform> builder)
    {
        builder.Property(platform => platform.Name).HasMaxLength(64);
        builder.Property(platform => platform.Alias).HasMaxLength(64);
        builder.Property(platform => platform.Description).HasMaxLength(64);
        builder.Property(platform => platform.Description).HasMaxLength(64);

        builder.UsePropertyAccessMode(PropertyAccessMode.PreferField);

        builder.HasMany(platform => platform.Credentials)
            .WithOne()
            .HasForeignKey(credential => credential.UtilityPaymentPlatformId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }
}