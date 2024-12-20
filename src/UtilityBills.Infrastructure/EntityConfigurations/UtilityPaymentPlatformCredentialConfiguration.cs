using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.Entities;

namespace UtilityBills.Infrastructure.EntityConfigurations;

public class UtilityPaymentPlatformCredentialConfiguration : IEntityTypeConfiguration<UtilityPaymentPlatformCredential>
{
    public void Configure(EntityTypeBuilder<UtilityPaymentPlatformCredential> builder)
    {
        builder.ComplexProperty(credential => credential.Email, propertyBuilder =>
        {
            propertyBuilder.Property(email => email.Value)
                .HasMaxLength(512)
                .HasColumnName(nameof(UtilityPaymentPlatformCredential.Email));
        });

        builder.ComplexProperty(credential => credential.Password, propertyBuilder =>
        {
            propertyBuilder.Property("Value")
                .HasColumnName(nameof(UtilityPaymentPlatformCredential.Password));
        });
    }
}