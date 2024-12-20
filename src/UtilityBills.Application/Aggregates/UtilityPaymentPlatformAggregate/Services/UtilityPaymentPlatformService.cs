using FluentResults;
using UtilityBills.Abstractions;
using UtilityBills.Aggregates;
using UtilityBills.Aggregates.UtilityPaymentPlatformAggregate;
using UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.Entities;
using UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.Errors;
using UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.Services;
using UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.Specifications;
using UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.ValueObjects;

namespace UtilityBills.Application.Aggregates.UtilityPaymentPlatformAggregate.Services;

public class UtilityPaymentPlatformService : IUtilityPaymentPlatformService
{
    private readonly IRepository<UtilityPaymentPlatform> _repository;
    private readonly Dictionary<UtilityPaymentPlatformType, ICredentialsValidator> _credentialValidators;

    public UtilityPaymentPlatformService(IRepository<UtilityPaymentPlatform> repository,
        IEnumerable<ICredentialsValidator> credentialValidators)
    {
        _repository = repository;
        _credentialValidators = credentialValidators.ToDictionary(validator => validator.PlatformType);
    }

    public async Task<Result<UtilityPaymentPlatformCredential>> AddCredentialToPlatformAsync(Guid platformId,
        Email email, Password password, string userId,
        CancellationToken ct = default)
    {
        var platform =
            await _repository.FirstOrDefaultAsync(new GetPlatformWithUserCredentialsSpecification(platformId, userId),
                ct);

        if (platform is null)
        {
            return new PlatformNotFoundError();
        }

        var credentialValidator = _credentialValidators.GetValueOrDefault(platform.PlatformType) ??
                                  throw new InvalidOperationException();

        var credential = await platform.AddCredentialAsync(email, password, userId, credentialValidator);
        if (credential.IsFailed)
        {
            return credential;
        }

        await _repository.UpdateAsync(platform, ct);

        await _repository.UnitOfWork.CommitAsync(ct);

        return credential;
    }

    public async Task<Result<UtilityPaymentPlatform>> GetPlatformAsync(Guid platformId, CancellationToken ct = default)
    {
        var result = await _repository.FirstOrDefaultAsync(new GetPlatformByIdSpecification(platformId), ct);

        if (result is null)
        {
            return new PlatformNotFoundError();
        }

        return result;
    }

    public async Task<IReadOnlyCollection<UtilityPaymentPlatform>> GetPlatformsForWaterMeterReadingsAsync(
        string userId, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);
        
        var result = await _repository.ListAsync(new GetPlatformsWithWaterMeterReadings(userId), ct);

        return result;
    }
}