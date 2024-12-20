using FluentResults;
using KvadoClient.Clients;
using UtilityBills.Aggregates;
using UtilityBills.Aggregates.UtilityPaymentPlatformAggregate;
using UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.ValueObjects;

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

    public async Task<Result> SendWaterMeterReadingsAsync(Email email, Password password, WaterMeterReadings hotWater,
        WaterMeterReadings coldWater, CancellationToken ct = default)
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

    public Task<Result<DateOnly>> GetLastDayWhenWaterMeterReadingsWereSent(Email email, Password password,
        CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public async Task<decimal?> GetDebtAsync(Email email, Password password, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(email);
        ArgumentNullException.ThrowIfNull(password);

        return await _kvadoHttpClient.GetDebtAsync(email.Value, password.GetUnprotected(_passwordProtector), ct);
    }
}