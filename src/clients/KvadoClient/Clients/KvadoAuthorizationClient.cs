using System.Net;
using KvadoClient.Exceptions;
using KvadoClient.Parsers;

namespace KvadoClient.Clients;

public interface IKvadoAuthorizationClient
{
    Task<string[]> LoginAsync(string email, string password, CancellationToken ct = default);
}

internal class KvadoAuthorizationClient : IKvadoAuthorizationClient
{
    private const string LoginUrl = "/login";

    private readonly HttpClient _httpClient;

    public KvadoAuthorizationClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<string[]> LoginAsync(string email, string password, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(email);
        ArgumentException.ThrowIfNullOrEmpty(password);

        using var loginPageRequest = await _httpClient.GetAsync(LoginUrl, ct);
        
        string loginPage = await loginPageRequest.Content.ReadAsStringAsync(ct);

        string token = new LoginPageParser().GetCsrfToken(loginPage);

        using var loginRequest = CreateLoginRequest(email, password, token);

        using var loginResponse = await _httpClient.SendAsync(loginRequest, ct);
        
        if (loginResponse.StatusCode != HttpStatusCode.Found)
        {
            throw new KvadoAuthenticationException("Unable to authenticate");
        }
        
        var cookie = loginResponse.Headers.GetValues("Set-Cookie").ToArray();
        
        return cookie;
    }

    private HttpRequestMessage CreateLoginRequest(string email,
        string password,
        string token)
    {
        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("YII_CSRF_TOKEN", token),
            new KeyValuePair<string, string>("LoginForm[email]", email),
            new KeyValuePair<string, string>("LoginForm[password]", password)
        });

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/login");

        requestMessage.Headers.Referrer = new Uri("https://cabinet.kvado.ru/login");

        requestMessage.Content = content;

        return requestMessage;
    }
}