using System.Collections.Concurrent;
using FluentResults;
using UtilityBills.Aggregates.ReadingPlatformAggregate;
using UtilityBills.Aggregates.ReadingPlatformAggregate.Entities;
using UtilityBills.Aggregates.ReadingPlatformAggregate.Models;
using UtilityBills.Aggregates.ReadingPlatformAggregate.Services;
using UtilityBills.Aggregates.ReadingPlatformAggregate.ValueObjects;

namespace UtilityBills.Application.Aggregates.ReadingPlatformAggregate.Services;

public class MeterReadingsService : IMeterReadingsService
{
    private readonly IReadingPlatformService _platformService;
    private readonly IOrientProvider _orientProvider;
    private readonly IKvadoProvider _kvadoProvider;

    public MeterReadingsService(IReadingPlatformService platformService, IOrientProvider orientProvider,
        IKvadoProvider kvadoProvider)
    {
        _platformService = platformService;
        _orientProvider = orientProvider;
        _kvadoProvider = kvadoProvider;
    }

    public async Task<Result<MeterReadingsPair?>> GetCurrentReadingsAsync(string userId,
        CancellationToken ct = default)
    {
        var result = await GetMeterReadingsAsync(userId, RequestReadingType.Current, ct);

        return result;
    }

    public async Task<Result<MeterReadingsPair?>> GetPreviousReadingsAsync(string userId,
        CancellationToken ct = default)
    {
        var previous = await GetMeterReadingsAsync(userId, RequestReadingType.Previous, ct);

        var current = await GetMeterReadingsAsync(userId, RequestReadingType.Current, ct);

        // for case when user already sent the new meter readings and he can't send meter readings greater than that
        var coldWater = current.Value?.ColdWater?.Value > previous.Value?.ColdWater.Value
            ? current.Value.ColdWater
            : previous.Value!.ColdWater;

        var hotWater = current.Value?.HotWater.Value > previous.Value?.HotWater?.Value
            ? current.Value.HotWater
            : previous.Value!.HotWater!;

        return Result.Ok(MeterReadingsPair.Create(hotWater, coldWater));
    }

    public async Task<Result> SendReadingsAsync(string userId, MeterReadings hotWater, MeterReadings coldWater,
        CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);
        ArgumentNullException.ThrowIfNull(hotWater);

        await ExecutePlatformActionAsync(userId,
            async (platform, credential, token) =>
            {
                await SendReadingsAsync(platform, credential, hotWater, coldWater, token);
            }, ct);

        return Result.Ok();
    }

    private async Task<Result<MeterReadingsPair?>> GetMeterReadingsAsync(string userId,
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

            if (readings is null)
            {
                return;
            }

            result.Add(readings);
        }, ct);

        var maxHotWater = result.MaxBy(pair => pair.HotWater.Value);
        var maxColdWater = result.Where(pair => pair.ColdWater is not null).MaxBy(pair => pair.ColdWater.Value);

        if (maxHotWater is null)
        {
            return Result.Fail("Unable to get hot water");
        }

        return Result.Ok(MeterReadingsPair.Create(maxHotWater.HotWater,
            maxColdWater is not null ? maxColdWater.ColdWater : null));
    }

    private async Task ExecutePlatformActionAsync(string userId,
        Func<ReadingPlatform, ReadingPlatformCredential, CancellationToken, Task> func,
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

    private async Task SendReadingsAsync(ReadingPlatform platform,
        ReadingPlatformCredential credential,
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
            ReadingPlatformType.Kvado => (await _kvadoProvider.SendMeterReadingsAsync(
                credential.Email, credential.Password, hotWater, coldWater, ct)),
            ReadingPlatformType.Orient => (await _orientProvider.SendMeterReadingsAsync(
                credential.Email, credential.Password, hotWater, ct)),
            ReadingPlatformType.RusEnergy => throw new NotSupportedException(),
            _ => throw new NotSupportedException()
        };
    }


    private async Task<MeterReadingsPair?> GetCurrentReadingsAsync(
        ReadingPlatform platform,
        ReadingPlatformCredential credential,
        CancellationToken ct)
    {
        return platform.PlatformType switch
        {
            ReadingPlatformType.Kvado => (await _kvadoProvider.GetCurrentMeterReadingsAsync(
                credential.Email, credential.Password, ct))?.Value,
            ReadingPlatformType.Orient => (await _orientProvider.GetPreviousWaterMeterReadingAsync(
                credential.Email, credential.Password, ct)).Value,
            ReadingPlatformType.RusEnergy => throw new NotSupportedException(),
            _ => throw new NotSupportedException()
        };
    }

    private async Task<MeterReadingsPair?> GetPreviousReadingsAsync(
        ReadingPlatform platform,
        ReadingPlatformCredential credential,
        CancellationToken ct)
    {
        return platform.PlatformType switch
        {
            ReadingPlatformType.Kvado => (await _kvadoProvider.GetPreviousMeterReadingsAsync(
                credential.Email, credential.Password, ct)).Value,
            ReadingPlatformType.Orient => (await _orientProvider.GetPreviousWaterMeterReadingAsync(
                credential.Email, credential.Password, ct)).Value,
            ReadingPlatformType.RusEnergy => throw new NotSupportedException(),
            _ => throw new NotSupportedException()
        };
    }


    private enum RequestReadingType
    {
        Current,
        Previous
    }
}