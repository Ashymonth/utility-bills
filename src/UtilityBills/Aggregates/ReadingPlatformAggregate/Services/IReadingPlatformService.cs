using FluentResults;
using UtilityBills.Aggregates.ReadingPlatformAggregate.Entities;
using UtilityBills.Aggregates.ReadingPlatformAggregate.ValueObjects;

namespace UtilityBills.Aggregates.ReadingPlatformAggregate.Services;

public interface IReadingPlatformService
{
    Task<Result<ReadingPlatformCredential>> AddCredentialToPlatformAsync(Guid platformId, Email email,
        Password password, string userId, CancellationToken ct = default);
 
    Task<List<ReadingPlatform>> GetPlatformsAsync(CancellationToken ct = default);

    Task<IReadOnlyCollection<ReadingPlatform>> GetPlatformsForMeterReadingsAsync(string userId,
        CancellationToken ct = default);
}