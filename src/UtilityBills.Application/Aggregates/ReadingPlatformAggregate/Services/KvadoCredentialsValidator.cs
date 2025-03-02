using FluentResults;
using UtilityBills.Aggregates;
using UtilityBills.Aggregates.ReadingPlatformAggregate;
using UtilityBills.Aggregates.ReadingPlatformAggregate.Entities;

namespace UtilityBills.Application.Aggregates.ReadingPlatformAggregate.Services;

public class KvadoCredentialsValidator : ICredentialsValidator
{
    private readonly IKvadoProvider _kvadoProvider;

    public KvadoCredentialsValidator(IKvadoProvider kvadoProvider)
    {
        _kvadoProvider = kvadoProvider;
    }

    public ReadingPlatformType PlatformType => ReadingPlatformType.Kvado;

    public async Task<Result<ReadingPlatformCredential>> ValidateAsync(
        ReadingPlatformCredential credential,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(credential);

        var result = await _kvadoProvider.AuthenticateAsync(credential.Email, credential.Password, ct);

        return result.IsSuccess ? credential : result;
    }
}