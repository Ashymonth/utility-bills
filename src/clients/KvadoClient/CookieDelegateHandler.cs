using KvadoClient.Exceptions;
using KvadoClient.Extensions;

namespace KvadoClient;

internal class CookieDelegateHandler : DelegatingHandler
{
    private const string CookieHeader = "Cookie";

    private readonly ICookieProvider _cookieProvider;

    public CookieDelegateHandler(ICookieProvider cookieProvider)
    {
        _cookieProvider = cookieProvider ?? throw new ArgumentNullException(nameof(cookieProvider));
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var (email, password) = request.GetAuthOption();

        string[] cookie;
        try
        {
            cookie = await _cookieProvider.GetAuthCookieAsync(email, password, cancellationToken);
        }
        catch (KvadoException) // login or password was incorrect, so we remove stored cookie.
        {
            _cookieProvider.CleanAuthCookie(email);
            throw;
        }
        
        request.Headers.TryAddWithoutValidation(CookieHeader, cookie);

        var result = await base.SendAsync(request, default);
        
        return result;
    }
}