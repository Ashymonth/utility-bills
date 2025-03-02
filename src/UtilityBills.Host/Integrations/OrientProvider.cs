using FluentResults;
using OrientClient.Clients;
using UtilityBills.Aggregates;
using UtilityBills.Aggregates.UtilityPaymentPlatformAggregate;
using UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.Models;
using UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.ValueObjects;

namespace UtilityBills.Host.Integrations;

internal class OrientProvider : IOrientProvider
{
    private readonly IOrientAuthorizationClient _authorizationClient;
    private readonly IPasswordProtector _passwordProtector;
    private readonly IOrientClient _orientClient;
    private readonly ILogger<OrientProvider>? _logger;

    public OrientProvider(IOrientAuthorizationClient authorizationClient, IPasswordProtector passwordProtector,
        IOrientClient orientClient, ILogger<OrientProvider>? logger = null)
    {
        _authorizationClient = authorizationClient;
        _passwordProtector = passwordProtector;
        _orientClient = orientClient;
        _logger = logger;
    }

    public async Task<Result> AuthenticateAsync(Email email, Password password, CancellationToken ct = default)
    {
        var plainPassword = password.GetUnprotected(_passwordProtector);

        try
        {
            await _authorizationClient.LoginAsync(email.Value, plainPassword, ct);
            return Result.Ok();
        }
        catch (Exception e)
        {
            _logger?.LogWarning(e, "Unable to authenticate in Orient platform with email:{OrientEmail}", email.Value);

            return Result.Fail("Login or password are incorrect or email don't exist");
        }
    }

    public async Task<Result> SendMeterReadingsAsync(Email email, Password password, MeterReadings hotWater,
        CancellationToken ct = default)
    {
        try
        {
            await _orientClient.SendWaterReadingsAsync(email.Value, password.GetUnprotected(_passwordProtector),
                hotWater.Value, ct);

            return Result.Ok();
        }
        catch (Exception e)
        {
            _logger?.LogWarning(e,
                "Unable to send hot water meter readings in Orient platform with email:{OrientEmail} and hot water: {HotWater}",
                email.Value, hotWater.Value);
            return Result.Fail("");
        }
    }

    public async Task<Result<DateOnly>> GetLastDayWhenMeterReadingsWereSent(Email email, Password password,
        CancellationToken ct = default)
    {
        try
        {
            var result = await _orientClient.LastDateWhenHotWaterReadingWereSentAsync(email.Value,
                password.GetUnprotected(_passwordProtector), ct);

            return result;
        }
        catch (Exception e)
        {
            _logger?.LogWarning(e,
                "Unable to get last date when water meter readings were sent for email:{OrientEmail}", email.Value);
            return Result.Fail("Unable to get last date when water meter readings were sent");
        }
    }

    public async Task<Result<MeterReadingsPair>> GetPreviousWaterMeterReadingAsync(Email email, Password password,
        CancellationToken ct = default)
    {
        try
        {
            var result = await _orientClient.GetPreviousWaterMeterReadingAsync(email.Value,
                password.GetUnprotected(_passwordProtector), ct);

            return Result.Ok(MeterReadingsPair.Create(MeterReadings.Create(result).Value, null));
        }
        catch (Exception e)
        {
            return Result.Fail("Unable to get prev hot water meter readings");
        }
    }

    public async Task<decimal?> GetDebtAsync(Email email, Password password, CancellationToken ct = default)
    {
        return await _orientClient.GetDebtAsync(email.Value, password.GetUnprotected(_passwordProtector), ct);
    }
}