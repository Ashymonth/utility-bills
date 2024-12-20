using FluentResults;
using UtilityBills.Aggregates.UtilityPaymentPlatformAggregate;
using UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.Services;
using UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.ValueObjects;

namespace UtilityBills.Application.Aggregates.UtilityPaymentPlatformAggregate.Services;

public class WaterMeterReadingsService : IWaterMeterReadingsService
{
    private readonly IUtilityPaymentPlatformService _platformService;
    private readonly IOrientProvider _orientProvider;
    private readonly IKvadoProvider _kvadoProvider;

    public WaterMeterReadingsService(IUtilityPaymentPlatformService platformService, IOrientProvider orientProvider,
        IKvadoProvider kvadoProvider)
    {
        _platformService = platformService;
        _orientProvider = orientProvider;
        _kvadoProvider = kvadoProvider;
    }

    public async Task<Result> SendAsync(string userId, WaterMeterReadings hotWater, WaterMeterReadings? coldWater,
        CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);
        ArgumentNullException.ThrowIfNull(hotWater);

        var platforms = await _platformService.GetPlatformsForWaterMeterReadingsAsync(userId, ct);
        if (platforms.Count == 0)
        {
            throw new InvalidOperationException("Platforms are not set");
        }
        
        if (platforms.All(platform => platform.Credentials.Count == 0))
        {
            return Result.Fail("Unable to send water meter readings without any credentials for platform");
        }

        var today = DateOnly.FromDateTime(DateTime.Now);
        if (platforms.All(platform => !platform.CanSendMeterReadings(today)))
        {
            return Result.Fail(
                "Unable to send water meter readings because there is no any platforms that allow to send it today");
        }

        foreach (var platform in platforms)
        {
            if (!platform.CanSendMeterReadings(today))
            {
                continue;
            }
            
            var credentialForPlatform = platform.GetUserCredential(userId);
            if (credentialForPlatform is null)
            {
                continue;
            }
            
            switch (platform.PlatformType)
            {
                // in the orient platform we can send only hot water meter readings
                case UtilityPaymentPlatformType.Orient:
                    await _orientProvider.SendWaterMeterReadingsAsync(credentialForPlatform.Email,
                        credentialForPlatform.Password, hotWater, ct);
                    break;
                // in the kvado platform we can send hot and cold water meter readings
                case UtilityPaymentPlatformType.Kvado when coldWater is not null:
                    await _kvadoProvider.SendWaterMeterReadingsAsync(credentialForPlatform.Email,
                        credentialForPlatform.Password, hotWater, coldWater, ct);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        return Result.Ok();
    }
}