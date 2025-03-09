using FluentResults;
using UtilityBills.Abstractions;
using UtilityBills.Aggregates;
using UtilityBills.Aggregates.ReadingPlatformAggregate;
using UtilityBills.Aggregates.ReadingPlatformAggregate.Entities;
using UtilityBills.Aggregates.ReadingPlatformAggregate.Errors;
using UtilityBills.Aggregates.ReadingPlatformAggregate.Services;
using UtilityBills.Aggregates.ReadingPlatformAggregate.Specifications;
using UtilityBills.Aggregates.ReadingPlatformAggregate.ValueObjects;

namespace UtilityBills.Application.Aggregates.ReadingPlatformAggregate.Services;

public class ReadingPlatformService : IReadingPlatformService
{
    private readonly IRepository<ReadingPlatform> _repository;
    private readonly Dictionary<ReadingPlatformType, ICredentialsValidator> _credentialValidators;

    public ReadingPlatformService(IRepository<ReadingPlatform> repository,
        IEnumerable<ICredentialsValidator> credentialValidators)
    {
        _repository = repository;
        _credentialValidators = credentialValidators.ToDictionary(validator => validator.PlatformType);
    }

    public async Task<List<ReadingPlatform>> GetPlatformsWithUserCredentialsAsync(string userId)
    {
        return await _repository.ListAsync(new GetAllPlatformsWithCredentials(userId));
    }
    

    public async Task<Result<ReadingPlatformCredential>> AddCredentialToPlatformAsync(Guid platformId,
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

        var credential = await platform.AddCredentialAsync(userId, email, password, credentialValidator);
        if (credential.IsFailed)
        {
            return credential;
        }

        await _repository.UpdateAsync(platform, ct);

        await _repository.UnitOfWork.CommitAsync(ct);

        return credential;
    }

    public async Task<List<ReadingPlatform>> GetPlatformsAsync(CancellationToken ct = default)
    {
        return await _repository.ListAsync(ct);
    }

    public async Task<Result<ReadingPlatform>> GetPlatformAsync(Guid platformId, CancellationToken ct = default)
    {
        var result = await _repository.FirstOrDefaultAsync(new GetPlatformByIdSpecification(platformId), ct);

        if (result is null)
        {
            return new PlatformNotFoundError();
        }

        return result;
    }

    public async Task<IReadOnlyCollection<ReadingPlatform>> GetPlatformsForMeterReadingsAsync(
        string userId, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);

        var result = await _repository.ListAsync(new GetPlatformsWithMeterReadings(userId), ct);

        return result;
    }
}