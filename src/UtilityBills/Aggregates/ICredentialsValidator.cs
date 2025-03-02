using FluentResults;
using UtilityBills.Aggregates.ReadingPlatformAggregate;
using UtilityBills.Aggregates.ReadingPlatformAggregate.Entities;

namespace UtilityBills.Aggregates;

public interface ICredentialsValidator
{
    ReadingPlatformType PlatformType { get; }

    Task<Result<ReadingPlatformCredential>> ValidateAsync(ReadingPlatformCredential credential,
        CancellationToken ct = default);
}