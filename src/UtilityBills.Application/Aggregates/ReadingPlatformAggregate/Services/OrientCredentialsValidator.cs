using FluentResults;
using UtilityBills.Aggregates;
using UtilityBills.Aggregates.ReadingPlatformAggregate;
using UtilityBills.Aggregates.ReadingPlatformAggregate.Entities;

namespace UtilityBills.Application.Aggregates.ReadingPlatformAggregate.Services;

public class OrientCredentialsValidator : ICredentialsValidator
{
    private readonly IOrientProvider _orientClient;

    public OrientCredentialsValidator(IOrientProvider orientClient)
    {
        _orientClient = orientClient;
    }

    public ReadingPlatformType PlatformType => ReadingPlatformType.Orient;

    public async Task<Result<ReadingPlatformCredential>> ValidateAsync(
        ReadingPlatformCredential credential, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(credential);
        
        var result = await _orientClient.AuthenticateAsync(credential.Email, credential.Password, ct);

        return result.IsSuccess ? credential : result;
    }
}