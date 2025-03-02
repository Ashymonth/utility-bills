using System.Collections.Concurrent;
using FluentResults;
using UtilityBills.Aggregates.UtilityPaymentPlatformAggregate;
using UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.Entities;
using UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.Models;
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

    public async Task<Result<WaterMeterReadingsPair>> GetCurrentReadingsAsync(string userId,
        CancellationToken ct = default)
    {
        var result = await GetWaterMeterReadingsAsync(userId, RequestReadingType.Current, ct);

        return result;
    }

    public async Task<Result<WaterMeterReadingsPair>> GetPreviousReadingsAsync(string userId,
        CancellationToken ct = default)
    {
        var result = await GetWaterMeterReadingsAsync(userId, RequestReadingType.Previous, ct);

        return result;
    }

    public async Task<Result> SendReadingsAsync(string userId, WaterMeterReadings hotWater, WaterMeterReadings coldWater,
        CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);
        ArgumentNullException.ThrowIfNull(hotWater);

        await ExecutePlatformActionAsync(userId, async (platform, credential, token) =>
            {
                await SendReadingsAsync(platform, credential, hotWater, coldWater, token);
            }, ct);

        return Result.Ok();
    }

    private async Task<Result<WaterMeterReadingsPair>> GetWaterMeterReadingsAsync(string userId,
        RequestReadingType requestReadingType, CancellationToken ct = default)
    {
        var result = new ConcurrentBag<WaterMeterReadingsPair>();
        await ExecutePlatformActionAsync(userId, async (platform, credentialForPlatform, token) =>
        {
            var readings = requestReadingType switch
            {
                RequestReadingType.Current => await GetCurrentReadingsAsync(platform, credentialForPlatform, token),
                RequestReadingType.Previous => await GetPreviousReadingsAsync(platform, credentialForPlatform, token),
                _ => throw new ArgumentOutOfRangeException(nameof(requestReadingType), requestReadingType, null)
            };

            result.Add(readings);
        }, ct);

        var maxHotWater = result.MaxBy(pair => pair.HotWater.Value);
        var maxColdWater = result.MaxBy(pair => pair.ColdWater?.Value);

        if (maxHotWater is null)
        {
            return Result.Fail("Unable to get hot water");
        }

        return Result.Ok(WaterMeterReadingsPair.Create(maxHotWater.HotWater, maxColdWater?.ColdWater));
    }

    private async Task ExecutePlatformActionAsync(string userId,
        Func<UtilityPaymentPlatform, UtilityPaymentPlatformCredential, CancellationToken, Task> func,
        CancellationToken ct = default)
    {
        var platforms = await _platformService.GetPlatformsForWaterMeterReadingsAsync(userId, ct);
        if (platforms.Count == 0)
        {
            throw new InvalidOperationException("Platforms are not set");
        }

        if (platforms.All(platform => platform.Credentials.Count == 0))
        {
            throw new InvalidOperationException("User dont have any credentials");
        }

        await Parallel.ForEachAsync(platforms, ct, async (platform, token) =>
        {
            var credentialForPlatform = platform.GetUserCredential(userId);
            if (credentialForPlatform is null)
            {
                return;
            }

            await func(platform, credentialForPlatform, token);
        });
    }

    private async Task SendReadingsAsync(UtilityPaymentPlatform platform,
        UtilityPaymentPlatformCredential credential,
        WaterMeterReadings hotWater,
        WaterMeterReadings coldWater,
        CancellationToken ct)
    {
        if (!platform.CanSendMeterReadings(DateOnly.FromDateTime(DateTime.Now)))
        {
            throw new InvalidOperationException("Platform cannot send readings");
        }

        _ = platform.PlatformType switch
        {
            UtilityPaymentPlatformType.Kvado => (await _kvadoProvider.SendWaterMeterReadingsAsync(
                credential.Email, credential.Password, hotWater, coldWater, ct)),
            UtilityPaymentPlatformType.Orient => (await _orientProvider.SendWaterMeterReadingsAsync(
                credential.Email, credential.Password, hotWater, ct)),
            UtilityPaymentPlatformType.RusEnergy => throw new NotSupportedException(),
            _ => throw new NotSupportedException()
        };
    }


    private async Task<WaterMeterReadingsPair> GetCurrentReadingsAsync(
        UtilityPaymentPlatform platform,
        UtilityPaymentPlatformCredential credential,
        CancellationToken ct)
    {
        return platform.PlatformType switch
        {
            UtilityPaymentPlatformType.Kvado => (await _kvadoProvider.GetCurrentWaterMeterReadingsAsync(
                credential.Email, credential.Password, ct)).Value,
            UtilityPaymentPlatformType.Orient => (await _orientProvider.GetPreviousWaterMeterReadingAsync(
                credential.Email, credential.Password, ct)).Value,
            UtilityPaymentPlatformType.RusEnergy => throw new NotSupportedException(),
            _ => throw new NotSupportedException()
        };
    }

    private async Task<WaterMeterReadingsPair> GetPreviousReadingsAsync(
        UtilityPaymentPlatform platform,
        UtilityPaymentPlatformCredential credential,
        CancellationToken ct)
    {
        return platform.PlatformType switch
        {
            UtilityPaymentPlatformType.Kvado => (await _kvadoProvider.GetPreviousWaterMeterReadingsAsync(
                credential.Email, credential.Password, ct)).Value,
            UtilityPaymentPlatformType.Orient => (await _orientProvider.GetPreviousWaterMeterReadingAsync(
                credential.Email, credential.Password, ct)).Value,
            UtilityPaymentPlatformType.RusEnergy => throw new NotSupportedException(),
            _ => throw new NotSupportedException()
        };
    }


    private enum RequestReadingType
    {
        Current,
        Previous
    }
}