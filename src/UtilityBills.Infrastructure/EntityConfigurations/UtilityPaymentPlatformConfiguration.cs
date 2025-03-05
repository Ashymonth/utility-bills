using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UtilityBills.Aggregates.ReadingPlatformAggregate;

namespace UtilityBills.Infrastructure.EntityConfigurations;

public class ReadingPlatformConfiguration : IEntityTypeConfiguration<ReadingPlatform>
{
    public void Configure(EntityTypeBuilder<ReadingPlatform> builder)
    {
        builder.Property(platform => platform.Name).HasMaxLength(64);
        builder.Property(platform => platform.Description).HasMaxLength(64);
        builder.Property(platform => platform.Description).HasMaxLength(64);

        builder.UsePropertyAccessMode(PropertyAccessMode.PreferField);

        builder.HasMany(platform => platform.Credentials)
            .WithOne()
            .HasForeignKey(credential => credential.ReadingPlatformId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }
}