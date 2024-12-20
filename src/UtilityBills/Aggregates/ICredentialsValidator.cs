using FluentResults;
using UtilityBills.Aggregates.UtilityPaymentPlatformAggregate;
using UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.Entities;

namespace UtilityBills.Aggregates;

public interface ICredentialsValidator
{
    UtilityPaymentPlatformType PlatformType { get; }

    Task<Result<UtilityPaymentPlatformCredential>> ValidateAsync(UtilityPaymentPlatformCredential credential,
        CancellationToken ct = default);
}