using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UtilityBills.Aggregates.ReadingPlatformAggregate.Entities;

namespace UtilityBills.Infrastructure.EntityConfigurations;

public class ReadingPlatformCredentialConfiguration : IEntityTypeConfiguration<ReadingPlatformCredential>
{
    public void Configure(EntityTypeBuilder<ReadingPlatformCredential> builder)
    {
        builder.ComplexProperty(credential => credential.Email, propertyBuilder =>
        {
            propertyBuilder.Property(email => email.Value)
                .HasMaxLength(512)
                .HasColumnName(nameof(ReadingPlatformCredential.Email));
        });

        builder.ComplexProperty(credential => credential.Password, propertyBuilder =>
        {
            propertyBuilder.Property("Value")
                .HasColumnName(nameof(ReadingPlatformCredential.Password));
        });
    }
}