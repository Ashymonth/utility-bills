using FluentResults;
using UtilityBills.Abstractions;
using UtilityBills.Aggregates.ReadingPlatformAggregate.Entities;
using UtilityBills.Aggregates.ReadingPlatformAggregate.Events;
using UtilityBills.Aggregates.ReadingPlatformAggregate.ValueObjects;
using UtilityBills.Exceptions;

namespace UtilityBills.Aggregates.ReadingPlatformAggregate;

/// <summary>
/// Information about the utility payment platform. 
/// </summary>
public class ReadingPlatform : Entity, IAggregateRoot
{
    private static readonly Dictionary<ReadingPlatformType, HashSet<int>> DaysWhenMeterReadingsCanBeSend = new()
    {
        [ReadingPlatformType.Kvado] = [..Enumerable.Range(15, 25)],
        [ReadingPlatformType.Orient] = [..Enumerable.Range(20, 25)],
    };
    
    private readonly List<ReadingPlatformCredential> _platformCredentials = [];

    /// <summary>
    /// Platform name.
    /// </summary>
    public string Name { get; private set; } = null!;

    /// <summary>
    /// An alias for the platform name.
    /// </summary>
    public string Alias { get; private set; } = null!;

    public ReadingPlatformType PlatformType { get; private set; }

    /// <summary>
    /// Short information about the platform.
    /// </summary>
    public string Description { get; private set; } = null!;

    /// <summary>
    /// Linked credentials to this platform.
    /// </summary>
    public IReadOnlyCollection<ReadingPlatformCredential> Credentials => _platformCredentials;

    public static ReadingPlatform Create(string name, ReadingPlatformType type, string description)
    {
        return new ReadingPlatform
        {
            Name = name,
            PlatformType = type,
            Alias = type.ToString().ToLower(),
            Description = description,
        };
    }
    
    public ReadingPlatformCredential? GetUserCredential(string userId)
    {
        return Credentials.FirstOrDefault(credential => credential.UserId == userId);
    }

    public bool CanSendMeterReadings(DateOnly today)
    {
        return DaysWhenMeterReadingsCanBeSend[PlatformType].Contains(today.Day);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="email"></param>
    /// <param name="password"></param>
    /// <param name="userId"></param>
    /// <param name="validator"></param>
    public async Task<Result<ReadingPlatformCredential>> AddCredentialAsync(Email email, Password password,
        string userId,
        ICredentialsValidator validator)
    {
        if (validator.PlatformType != PlatformType)
        {
            throw new InvalidOperationException(
                $"For platform with type: {PlatformType} provided invalid credential validator because validator type is {validator.PlatformType}");
        }

        if (Credentials.Any(credential => credential.Email == email && credential.UserId == userId))
        {
            throw new ItemExistException(
                $"For platform: {PlatformType} with email : {email.Value} already exist credential");
        }

        var credentialResult = await ReadingPlatformCredential.CreateAsync(email, password, userId, Id, validator);
        if (credentialResult.IsFailed)
        {
            return credentialResult;
        }
        
        _platformCredentials.Add(credentialResult.Value);

        AddDomainEvent(new ReadingPlatformCredentialAddedEvent(credentialResult.Value));

        return credentialResult;
    }

    public void DeleteCredentials(Email email, string userId)
    {
        var credentialToDelete =
            Credentials.FirstOrDefault(credential => credential.Email == email && credential.UserId == userId);

        if (credentialToDelete is null)
        {
            throw new ItemNotFoundException(
                $"Credential with email: {email.Value} for platform: {PlatformType} not found");
        }

        _platformCredentials.Remove(credentialToDelete);
    }
}