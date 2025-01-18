using FluentResults;
using UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.Models;
using UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.ValueObjects;

namespace UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.Services;

public interface IWaterMeterReadingsService
{
    Task<Result<WaterMeterReadingsPair>> GetUserPlatformCredentialsAsync(string userId, CancellationToken ct = default);
    
    Task<Result> SendAsync(string userId, WaterMeterReadings hotWater, WaterMeterReadings? coldWater,
        CancellationToken ct = default);
}