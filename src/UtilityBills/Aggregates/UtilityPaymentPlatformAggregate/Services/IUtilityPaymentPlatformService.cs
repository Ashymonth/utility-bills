using FluentResults;
using UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.Entities;
using UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.ValueObjects;

namespace UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.Services;

public interface IUtilityPaymentPlatformService
{
    Task<Result<UtilityPaymentPlatformCredential>> AddCredentialToPlatformAsync(Guid platformId, Email email,
        Password password, string userId, CancellationToken ct = default);

    Task<Result<UtilityPaymentPlatform>> GetPlatformAsync(Guid platformId, CancellationToken ct = default);

    Task<IReadOnlyCollection<UtilityPaymentPlatform>> GetPlatformsForMeterReadingsAsync(string userId,
        CancellationToken ct = default);
}