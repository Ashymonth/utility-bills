using FluentResults;
using KvadoClient.Clients;
using UtilityBills.Aggregates;
using UtilityBills.Aggregates.ReadingPlatformAggregate;
using UtilityBills.Aggregates.ReadingPlatformAggregate.Models;
using UtilityBills.Aggregates.ReadingPlatformAggregate.ValueObjects;

namespace UtilityBills.Host.Integrations;

internal class KvadoProvider : IKvadoProvider
{
    private readonly IKvadoAuthorizationClient _authorizationClient;
    private readonly IKvadoHttpClient _kvadoHttpClient;
    private readonly IPasswordProtector _passwordProtector;
    private readonly ILogger<KvadoProvider>? _logger;

    public KvadoProvider(IKvadoAuthorizationClient authorizationClient, IKvadoHttpClient kvadoHttpClient,
        IPasswordProtector passwordProtector, ILogger<KvadoProvider>? logger = null)
    {
        _authorizationClient = authorizationClient;
        _kvadoHttpClient = kvadoHttpClient;
        _passwordProtector = passwordProtector;
        _logger = logger;
    }

    public async Task<Result> AuthenticateAsync(Email email, Password password, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(email);
        ArgumentNullException.ThrowIfNull(password);
        var plainPassword = password.GetUnprotected(_passwordProtector);

        try
        {
            await _authorizationClient.LoginAsync(email.Value, plainPassword, ct);
            return Result.Ok();
        }
        catch (Exception e)
        {
            _logger?.LogWarning(e, "Unable to authenticate in Kvado platform with email:{KvadoEmail}", email.Value);

            return Result.Fail("Login or password are incorrect or email don't exist");
        }
    }

    public async Task<Result> SendMeterReadingsAsync(Email email, Password password, MeterReadings hotWater,
        MeterReadings coldWater, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(email);
        ArgumentNullException.ThrowIfNull(password);
        ArgumentNullException.ThrowIfNull(hotWater);
        ArgumentNullException.ThrowIfNull(coldWater);

        try
        {
            await _kvadoHttpClient.SendWaterReadingsAsync(email.Value, password.GetUnprotected(_passwordProtector),
                hotWater.Value, coldWater.Value, ct);

            return Result.Ok();
        }
        catch (Exception e)
        {
            _logger?.LogWarning(e,
                "Unable to send how water meter readings in Kvado platform with email:{OrientEmail}, hot water: {HotWater}, cold water: {ColdWater}",
                email.Value, hotWater.Value, coldWater.Value);
            return Result.Fail("");
        }
    }

    public async Task<Result<MeterReadingsPair?>> GetPreviousMeterReadingsAsync(Email email, Password password,
        CancellationToken ct = default)
    {
        try
        {
            var result = await _kvadoHttpClient.GetPreviousMeterReadingsAsync(email.Value,
                password.GetUnprotected(_passwordProtector), ct);

            var hotWater = MeterReadings.Create(result.HotWater);
            var coldWater = MeterReadings.Create(result.ColdWater);

            if (hotWater.IsFailed || coldWater.IsFailed)
            {
                return Result.Fail("Unable to get previous water meter readings");
            }

            return Result.Ok(MeterReadingsPair.Create(hotWater.Value, coldWater.Value));
        }
        catch (Exception e)
        {
            _logger?.LogError(e, "Unable to get previouse water meter readings");
            return Result.Fail(e.Message);
        }
    }
    
    public async Task<Result<MeterReadingsPair?>?> GetCurrentMeterReadingsAsync(Email email, Password password,
        CancellationToken ct = default)
    {
        try
        {
            var result = await _kvadoHttpClient.GetCurrentMeterReadingsAsync(email.Value,
                password.GetUnprotected(_passwordProtector), ct);

            if (result is null)
            {
                return null;
            }
            
            var hotWater = MeterReadings.Create(result.HotWater);
            var coldWater = MeterReadings.Create(result.ColdWater);

            if (hotWater.IsFailed || coldWater.IsFailed)
            {
                return Result.Fail("Unable to get previous water meter readings");
            }

            return Result.Ok(MeterReadingsPair.Create(hotWater.Value, coldWater.Value));
        }
        catch (Exception e)
        {
            _logger?.LogError(e, "Unable to get previous water meter readings");
            return Result.Fail(e.Message);
        }
    }

    public async Task<decimal?> GetDebtAsync(Email email, Password password, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(email);
        ArgumentNullException.ThrowIfNull(password);

        return await _kvadoHttpClient.GetDebtAsync(email.Value, password.GetUnprotected(_passwordProtector), ct);
    }
}