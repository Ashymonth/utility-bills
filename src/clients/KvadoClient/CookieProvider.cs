using KvadoClient.Clients;
using Microsoft.Extensions.Caching.Memory;

namespace KvadoClient;

/// <summary>
/// Provide access to cookies.
/// </summary>
public interface ICookieProvider
{
    /// <summary>
    /// Get user authentication cookie.
    /// </summary>
    /// <param name="email">User email.</param>
    /// <param name="password">User password.</param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<string[]> GetAuthCookieAsync(string email, string password, CancellationToken ct = default);
    
    /// <summary>
    /// Remove auth cookie for selected user email.
    /// </summary>
    /// <param name="email">Email to remove cookie.</param>
    void CleanAuthCookie(string email);
}

/// <summary>
/// <see cref="ICookieProvider"/>
/// </summary>
internal class CookieProvider : ICookieProvider
{
    private const string CacheKeyTemplate = "kvado_{0}";
    
    private static readonly TimeSpan CookieCacheTime = TimeSpan.FromHours(1);

    private readonly IMemoryCache _cache;
    private readonly IKvadoAuthorizationClient _authorizationClient;

    public CookieProvider(IMemoryCache cache, IKvadoAuthorizationClient authorizationClient)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _authorizationClient = authorizationClient ?? throw new ArgumentNullException(nameof(authorizationClient));
    }

    public async Task<string[]> GetAuthCookieAsync(string email, string password, CancellationToken ct = default)
    {
        var cacheKey = string.Format(CacheKeyTemplate, email);
        if (_cache.TryGetValue(cacheKey, out string[]? cookie))
        {
            return cookie!;
        }

        cookie = await _authorizationClient.LoginAsync(email, password, ct);

        _cache.Set(cacheKey, cookie, CookieCacheTime);

        return cookie;
    }

    public void CleanAuthCookie(string email) => _cache.Remove(email);
}