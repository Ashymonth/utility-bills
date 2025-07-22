using FluentResults;
using UtilityBills.Aggregates.ReadingPlatformAggregate.Models;
using UtilityBills.Aggregates.ReadingPlatformAggregate.ValueObjects;

namespace UtilityBills.Aggregates.ReadingPlatformAggregate.Services;

public interface IMeterReadingsService
{
    Task<Result<MeterReadingsPair?>> GetCurrentReadingsAsync(string userId,
        CancellationToken ct = default);
    
    Task<Result<MeterReadingsPair?>> GetPreviousReadingsAsync(string userId, CancellationToken ct = default);
    
    Task<Result> SendReadingsAsync(string userId, MeterReadings hotWater, MeterReadings coldWater,
        CancellationToken ct = default);
}