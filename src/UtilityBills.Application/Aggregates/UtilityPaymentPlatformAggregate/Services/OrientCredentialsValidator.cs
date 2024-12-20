using FluentResults;
using UtilityBills.Aggregates;
using UtilityBills.Aggregates.UtilityPaymentPlatformAggregate;
using UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.Entities;

namespace UtilityBills.Application.Aggregates.UtilityPaymentPlatformAggregate.Services;

public class OrientCredentialsValidator : ICredentialsValidator
{
    private readonly IOrientProvider _orientClient;

    public OrientCredentialsValidator(IOrientProvider orientClient)
    {
        _orientClient = orientClient;
    }

    public UtilityPaymentPlatformType PlatformType => UtilityPaymentPlatformType.Orient;

    public async Task<Result<UtilityPaymentPlatformCredential>> ValidateAsync(
        UtilityPaymentPlatformCredential credential, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(credential);
        
        var result = await _orientClient.AuthenticateAsync(credential.Email, credential.Password, ct);

        return result.IsSuccess ? credential : result;
    }
}