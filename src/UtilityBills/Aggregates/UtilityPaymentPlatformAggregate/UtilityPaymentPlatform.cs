using FluentResults;
using UtilityBills.Abstractions;
using UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.Entities;
using UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.Events;
using UtilityBills.Aggregates.UtilityPaymentPlatformAggregate.ValueObjects;
using UtilityBills.Exceptions;

namespace UtilityBills.Aggregates.UtilityPaymentPlatformAggregate;

/// <summary>
/// Information about the utility payment platform. 
/// </summary>
public class UtilityPaymentPlatform : Entity, IAggregateRoot
{
    private static readonly Dictionary<UtilityPaymentPlatformType, HashSet<int>> DaysWhenMeterReadingsCanBeSend = new()
    {
        [UtilityPaymentPlatformType.Kvado] = [..Enumerable.Range(15, 25)],
        [UtilityPaymentPlatformType.Orient] = [..Enumerable.Range(20, 25)],
    };
    
    private readonly List<UtilityPaymentPlatformCredential> _platformCredentials = [];

    /// <summary>
    /// Platform name.
    /// </summary>
    public string Name { get; private set; } = null!;

    /// <summary>
    /// An alias for the platform name.
    /// </summary>
    public string Alias { get; private set; } = null!;

    public UtilityPaymentPlatformType PlatformType { get; private set; }

    /// <summary>
    /// Short information about the platform.
    /// </summary>
    public string Description { get; private set; } = null!;

    /// <summary>
    /// Linked credentials to this platform.
    /// </summary>
    public IReadOnlyCollection<UtilityPaymentPlatformCredential> Credentials => _platformCredentials;

    public static UtilityPaymentPlatform Create(string name, UtilityPaymentPlatformType type, string description)
    {
        return new UtilityPaymentPlatform
        {
            Name = name,
            PlatformType = type,
            Alias = type.ToString().ToLower(),
            Description = description,
        };
    }
    
    public UtilityPaymentPlatformCredential? GetUserCredential(string userId)
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
    public async Task<Result<UtilityPaymentPlatformCredential>> AddCredentialAsync(Email email, Password password,
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

        var credentialResult = await UtilityPaymentPlatformCredential.CreateAsync(email, password, userId, Id, validator);
        if (credentialResult.IsFailed)
        {
            return credentialResult;
        }
        
        _platformCredentials.Add(credentialResult.Value);

        AddDomainEvent(new UtilityPaymentPlatformCredentialAddedEvent(credentialResult.Value));

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