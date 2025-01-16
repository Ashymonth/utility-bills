using System.ComponentModel;
using Microsoft.Extensions.Caching.Memory;
using OrientClient.Clients;
using OrientClient.Contracts;

namespace OrientClient;

/// <summary>
/// Provide access to user cookie.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public interface IUserProvider
{
    /// <summary>
    /// Get user authentication cookie.
    /// </summary>
    /// <param name="email">User email.</param>
    /// <param name="password">User password.</param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<string[]> GetAndCacheAuthCookieAsync(string email, string password, CancellationToken ct = default);

    /// <summary>
    /// Remove auth cookie for selected user email.
    /// </summary>
    /// <param name="email">Email to remove cookie.</param>
    void CleanAuthCookie(string email);

    /// <summary>
    /// Get account id by email. User must be already logged.
    /// </summary>
    /// <param name="email"></param>
    /// <param name="password"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<string> GetAccountIdAsync(string email, string password, CancellationToken ct = default);
}

/// <summary>
/// <see cref="IUserProvider"/>
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
internal class UserProvider : IUserProvider
{
    private static readonly TimeSpan CookieCacheTime = TimeSpan.FromHours(1);

    private readonly IMemoryCache _cache;
    private readonly IOrientAuthorizationClient _authorizationClient;

    public UserProvider(IMemoryCache cache, IOrientAuthorizationClient authorizationClient)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _authorizationClient = authorizationClient ?? throw new ArgumentNullException(nameof(authorizationClient));
    }

    public async Task<string[]> GetAndCacheAuthCookieAsync(string email, string password, CancellationToken ct = default)
    {
        if (_cache.TryGetValue(email, out UserInfo? user))
        {              
            return user!.Cookie;
        }

        var loginResponse = await _authorizationClient.LoginAsync(email, password, ct);

        _cache.Set(email, loginResponse, CookieCacheTime);

        return loginResponse.Cookie;
    }

    public void CleanAuthCookie(string email) => _cache.Remove(email);

    public async Task<string> GetAccountIdAsync(string email, string password, CancellationToken ct = default)
    {
        await GetAndCacheAuthCookieAsync(email, password, ct);

        if (!_cache.TryGetValue(email, out UserInfo? response))
        {
            throw new InvalidOperationException($"User with email: {email} not logged");
        }

        return response!.AccountId;
    }
}