using FluentResults;
using UtilityBills.Abstractions;
using UtilityBills.Aggregates.ReadingPlatformAggregate.Entities;
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

    public ReadingPlatformType PlatformType { get; private init; }

    /// <summary>
    /// Short information about the platform.
    /// </summary>
    public string Description { get; private set; } = null!;

    /// <summary>
    /// Linked credentials to this platform.
    /// </summary>
    public IReadOnlyCollection<ReadingPlatformCredential> Credentials => _platformCredentials;

    public static ReadingPlatform Create(string name, ReadingPlatformType type)
    {
        return new ReadingPlatform
        {
            Name = name,
            PlatformType = type
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
    public async Task<Result<ReadingPlatformCredential>> AddCredentialAsync(
        string userId,
        Email email, Password password,
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

        return credentialResult;
    }
}