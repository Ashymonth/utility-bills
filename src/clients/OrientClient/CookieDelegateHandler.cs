using OrientClient.Exceptions;
using OrientClient.Extensions;

namespace OrientClient;

/// <summary>
/// Allows you to set in the header authenticating cookie for each request invoked with this handler. 
/// </summary>
internal class CookieDelegateHandler : DelegatingHandler
{
    private const string CookieHeader = "Cookie";

    private readonly IUserProvider _userProvider;

    public CookieDelegateHandler(IUserProvider userProvider)
    {
        _userProvider = userProvider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var (email, password) = request.GetAuthOption();
        
        if (string.IsNullOrWhiteSpace(email) && string.IsNullOrWhiteSpace(password))
        {
            return await base.SendAsync(request, cancellationToken);
        }
        
        string[] cookie;
        try
        {
            cookie = await _userProvider.GetAndCacheAuthCookieAsync(email!, password!, cancellationToken);
        }
        catch (OrientAuthenticationException) // login or password was incorrect, so we remove stored cookie.
        {
            _userProvider.CleanAuthCookie(email!);
            throw;
        }

        request.Headers.TryAddWithoutValidation(CookieHeader, cookie);

        return await base.SendAsync(request, cancellationToken);
    }
}