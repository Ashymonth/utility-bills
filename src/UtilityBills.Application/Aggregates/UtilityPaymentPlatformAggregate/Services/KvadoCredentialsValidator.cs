using FluentResults;
using UtilityBills.Aggregates;
using UtilityBills.Aggregates.UtilityPaymentPlatformAggregate;
using UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.Entities;

namespace UtilityBills.Application.Aggregates.UtilityPaymentPlatformAggregate.Services;

public class KvadoCredentialsValidator : ICredentialsValidator
{
    private readonly IKvadoProvider _kvadoProvider;

    public KvadoCredentialsValidator(IKvadoProvider kvadoProvider)
    {
        _kvadoProvider = kvadoProvider;
    }

    public UtilityPaymentPlatformType PlatformType => UtilityPaymentPlatformType.Kvado;

    public async Task<Result<UtilityPaymentPlatformCredential>> ValidateAsync(
        UtilityPaymentPlatformCredential credential,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(credential);

        var result = await _kvadoProvider.AuthenticateAsync(credential.Email, credential.Password, ct);

        return result.IsSuccess ? credential : result;
    }
}