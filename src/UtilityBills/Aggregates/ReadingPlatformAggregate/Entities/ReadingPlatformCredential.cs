using FluentResults;
using UtilityBills.Abstractions;
using UtilityBills.Aggregates.ReadingPlatformAggregate.ValueObjects;

namespace UtilityBills.Aggregates.ReadingPlatformAggregate.Entities;

/// <summary>
/// Credential to access the utility payment platform.
/// </summary>
public class ReadingPlatformCredential : Entity
{
    // For ef
    // ReSharper disable once UnusedMember.Local
    private ReadingPlatformCredential()
    {
        
    }
    
    private ReadingPlatformCredential(Email email, Password password, Guid platformId, string userId)
    {
        Email = email;
        Password = password;
        ReadingPlatformId = platformId;
        UserId = userId;
    }

    /// <summary>
    /// Credential email.
    /// </summary>
    public Email Email { get; private set; } = null!;

    /// <summary>
    /// Password for credentials.
    /// </summary>
    public Password Password { get; private set; } = null!;

    /// <summary>
    /// Platform identifier which this credential is related.
    /// </summary>
    public Guid ReadingPlatformId { get; private set; }

    /// <summary>
    /// User identifier which this credential is related.
    /// </summary>
    public string UserId { get; private set; } = null!;

    /// <summary>
    /// Create a new credentials.
    /// </summary>
    /// <param name="email"></param>
    /// <param name="password"></param>
    /// <param name="userId"></param>   
    /// <param name="platformId"></param>
    /// <param name="validator"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static async Task<Result<ReadingPlatformCredential>> CreateAsync(Email email, Password password,
        string userId, Guid platformId, ICredentialsValidator validator)
    {
        ArgumentNullException.ThrowIfNull(email);
        ArgumentNullException.ThrowIfNull(password);
        ArgumentNullException.ThrowIfNull(validator);
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);

        if (platformId == Guid.Empty)
        {
            throw new ArgumentException("The utility platform id can't be empty", nameof(platformId));
        }

        var credentials = new ReadingPlatformCredential(email, password, platformId, userId);

        return await validator.ValidateAsync(credentials);
    }
}