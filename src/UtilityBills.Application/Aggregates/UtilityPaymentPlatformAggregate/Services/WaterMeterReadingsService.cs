using System.Collections.Concurrent;
using FluentResults;
using UtilityBills.Aggregates.UtilityPaymentPlatformAggregate;
using UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.Entities;
using UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.Models;
using UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.Services;
using UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.ValueObjects;

namespace UtilityBills.Application.Aggregates.UtilityPaymentPlatformAggregate.Services;

public class MeterReadingsService : IMeterReadingsService
{
    private readonly IUtilityPaymentPlatformService _platformService;
    private readonly IOrientProvider _orientProvider;
    private readonly IKvadoProvider _kvadoProvider;

    public MeterReadingsService(IUtilityPaymentPlatformService platformService, IOrientProvider orientProvider,
        IKvadoProvider kvadoProvider)
    {
        _platformService = platformService;
        _orientProvider = orientProvider;
        _kvadoProvider = kvadoProvider;
    }

    public async Task<Result<MeterReadingsPair>> GetCurrentReadingsAsync(string userId,
        CancellationToken ct = default)
    {
        var result = await GetMeterReadingsAsync(userId, RequestReadingType.Current, ct);

        return result;
    }

    public async Task<Result<MeterReadingsPair>> GetPreviousReadingsAsync(string userId,
        CancellationToken ct = default)
    {
        var result = await GetMeterReadingsAsync(userId, RequestReadingType.Previous, ct);

        return result;
    }

    public async Task<Result> SendReadingsAsync(string userId, MeterReadings hotWater, MeterReadings coldWater,
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

    private async Task<Result<MeterReadingsPair>> GetMeterReadingsAsync(string userId,
        RequestReadingType requestReadingType, CancellationToken ct = default)
    {
        var result = new ConcurrentBag<MeterReadingsPair>();
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

        return Result.Ok(MeterReadingsPair.Create(maxHotWater.HotWater, maxColdWater?.ColdWater));
    }

    private async Task ExecutePlatformActionAsync(string userId,
        Func<UtilityPaymentPlatform, UtilityPaymentPlatformCredential, CancellationToken, Task> func,
        CancellationToken ct = default)
    {
        var platforms = await _platformService.GetPlatformsForMeterReadingsAsync(userId, ct);
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
        MeterReadings hotWater,
        MeterReadings coldWater,
        CancellationToken ct)
    {
        if (!platform.CanSendMeterReadings(DateOnly.FromDateTime(DateTime.Now)))
        {
            throw new InvalidOperationException("Platform cannot send readings");
        }

        _ = platform.PlatformType switch
        {
            UtilityPaymentPlatformType.Kvado => (await _kvadoProvider.SendMeterReadingsAsync(
                credential.Email, credential.Password, hotWater, coldWater, ct)),
            UtilityPaymentPlatformType.Orient => (await _orientProvider.SendMeterReadingsAsync(
                credential.Email, credential.Password, hotWater, ct)),
            UtilityPaymentPlatformType.RusEnergy => throw new NotSupportedException(),
            _ => throw new NotSupportedException()
        };
    }


    private async Task<MeterReadingsPair> GetCurrentReadingsAsync(
        UtilityPaymentPlatform platform,
        UtilityPaymentPlatformCredential credential,
        CancellationToken ct)
    {
        return platform.PlatformType switch
        {
            UtilityPaymentPlatformType.Kvado => (await _kvadoProvider.GetCurrentMeterReadingsAsync(
                credential.Email, credential.Password, ct)).Value,
            UtilityPaymentPlatformType.Orient => (await _orientProvider.GetPreviousWaterMeterReadingAsync(
                credential.Email, credential.Password, ct)).Value,
            UtilityPaymentPlatformType.RusEnergy => throw new NotSupportedException(),
            _ => throw new NotSupportedException()
        };
    }

    private async Task<MeterReadingsPair> GetPreviousReadingsAsync(
        UtilityPaymentPlatform platform,
        UtilityPaymentPlatformCredential credential,
        CancellationToken ct)
    {
        return platform.PlatformType switch
        {
            UtilityPaymentPlatformType.Kvado => (await _kvadoProvider.GetPreviousMeterReadingsAsync(
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