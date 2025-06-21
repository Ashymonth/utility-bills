using OrientClient.Contracts;
using OrientClient.Parsers;

namespace OrientClient.Clients;

/// <summary>
/// Client to authorize on https://lk.hppp.ru.
/// </summary>
public interface IOrientAuthorizationClient
{
    /// <summary>
    /// Login to the site.
    /// </summary>
    /// <param name="email">Email to dashboard.</param>
    /// <param name="password">Password to dashboard.</param>
    /// <param name="ct"></param>
    /// <returns>Return auth cookie if success login on site.</returns>
    Task<UserInfo> LoginAsync(string email, string password, CancellationToken ct = default);
}

/// <summary>
/// <see cref="IOrientAuthorizationClient"/>
/// </summary>
internal class OrientAuthorizationClient : IOrientAuthorizationClient
{
    private const string CookieHeaderKey = "Set-Cookie";
    private const string LoginUrl = "/login";
    
    private readonly HttpClient _httpClient;

    public OrientAuthorizationClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<UserInfo> LoginAsync(string email, string password, CancellationToken ct = default)
    {
        using var loginPageResponse = await _httpClient.GetAsync(LoginUrl, ct);

        string loginPage = await loginPageResponse.Content.ReadAsStringAsync(ct);

        string[] cookie = loginPageResponse.Headers.GetValues(CookieHeaderKey).ToArray();

        var parser = new LoginPageParser();
        string token = parser.GetCsrfToken(loginPage);

        var captchaToken = parser.GetDefaultCaptchaToken(loginPage); 

        using var loginRequest = CreateLoginRequest(email, password, token,captchaToken, cookie);

        string accountPage = await LoginAsync(loginRequest, ct); // will redirect to account page on success.

        string accountId = new AccountIdParser().GetAccountId(accountPage);

        return new UserInfo
        {
            AccountId = accountId,
            Cookie = cookie
        };
    }

    private async Task<string> LoginAsync(HttpRequestMessage loginRequest, CancellationToken ct)
    {
        using var loginResponse = await _httpClient.SendAsync(loginRequest, ct);

        loginResponse.EnsureSuccessStatusCode();

        return await loginResponse.Content.ReadAsStringAsync(ct);
    }

    private static HttpRequestMessage CreateLoginRequest(string email,
        string password,
        string token,
        string captchaToken,
        IEnumerable<string> cookie)
    {
        var content = new FormUrlEncodedContent([
            new KeyValuePair<string, string>("login", email),
            new KeyValuePair<string, string>("password", password),
            new KeyValuePair<string, string>("_token", token),
            new KeyValuePair<string, string>("default-captcha-response", captchaToken),
            new KeyValuePair<string, string>("page-code", "authorization"),
            new KeyValuePair<string, string>("agreement", "on")
        ]);

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/login");

        requestMessage.Headers.TryAddWithoutValidation(CookieHeaderKey, cookie);

        requestMessage.Content = content;

        return requestMessage;
    }
}