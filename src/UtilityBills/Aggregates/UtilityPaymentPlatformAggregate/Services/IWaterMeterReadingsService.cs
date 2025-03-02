using FluentResults;
using UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.Models;
using UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.ValueObjects;

namespace UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.Services;

public interface IMeterReadingsService
{
    Task<Result<MeterReadingsPair>> GetCurrentReadingsAsync(string userId,
        CancellationToken ct = default);
    
    Task<Result<MeterReadingsPair>> GetPreviousReadingsAsync(string userId, CancellationToken ct = default);
    
    Task<Result> SendReadingsAsync(string userId, MeterReadings hotWater, MeterReadings coldWater,
        CancellationToken ct = default);
}