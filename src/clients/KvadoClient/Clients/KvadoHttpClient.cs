using System.Globalization;
using KvadoClient.Exceptions;
using KvadoClient.Extensions;
using KvadoClient.Models;
using KvadoClient.Parsers;
using Microsoft.Extensions.Logging;

namespace KvadoClient.Clients;

public interface IKvadoHttpClient
{
    /// <summary>
    /// Send counters data.
    /// </summary>
    /// <param name="email">User email.</param>
    /// <param name="password">User password.</param>
    /// <param name="hotWater">Hot water value.</param>
    /// <param name="coldWater">Cold water value.</param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task SendWaterReadingsAsync(string email,
        string password,
        decimal hotWater,
        decimal coldWater,
        CancellationToken ct = default);

    /// <summary>
    /// Get user debt.
    /// </summary>
    /// <param name="email">User email to log in.</param>
    /// <param name="password">Use the password to log in.</param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<decimal?> GetDebtAsync(string email, string password, CancellationToken ct = default);

    Task<PreviousWaterMeterReadings> GetPreviousWaterMeterReadingsAsync(string email, string password,
        CancellationToken ct = default);
}

public class KvadoHttpClient : IKvadoHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<KvadoHttpClient> _logger;

    public KvadoHttpClient(HttpClient httpClient, ILogger<KvadoHttpClient> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger;
    }

    public async Task SendWaterReadingsAsync(string email,
        string password,
        decimal hotWater,
        decimal coldWater,
        CancellationToken ct = default)
    {
        var csrfToken = await GetCsrfTokenFromCountersPageAsync(email, password, ct);
        var (coldWaterMeterId, howWaterMeterId) = await GetMeterIdsAsync(email, password, ct);

        using var sendCountersRequest = new HttpRequestMessage(HttpMethod.Post, "/counters");
        sendCountersRequest.SetAuthOptions(email, password);
        sendCountersRequest.Headers.Referrer = new Uri("https://cabinet.kvado.ru/counters");

        sendCountersRequest.Content = CreateSendCountersRequestBody(csrfToken,
            new Meter { MeterId = howWaterMeterId, Value = hotWater },
            new Meter { MeterId = coldWaterMeterId, Value = coldWater });

        using var sendCountersResponse = await _httpClient.SendAsync(sendCountersRequest, ct);

        sendCountersResponse.EnsureSuccessStatusCode();
    }

    public async Task<decimal?> GetDebtAsync(string email, string password, CancellationToken ct = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/house");
        request.SetAuthOptions(email, password);

        using var response = await _httpClient.SendAsync(request, ct);

        var content = await response.Content.ReadAsStringAsync(ct);

        return new DebtParser().ParseDebt(content);
    }

    public async Task<PreviousWaterMeterReadings> GetPreviousWaterMeterReadingsAsync(string email, string password,
        CancellationToken ct = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/counters");
        request.SetAuthOptions(email, password);
        
        using var response = await _httpClient.SendAsync(request, ct);
        
        var page = await response.Content.ReadAsStringAsync(ct);

        return new PreviousWaterMeterReadingsParser().ParsePreviousWaterMeterReadings(page);
    }

    private static FormUrlEncodedContent CreateSendCountersRequestBody(string token,
        Meter hotWater,
        Meter coldWater)
    {
        var requestContent = new FormUrlEncodedContent([
            new KeyValuePair<string, string>("YII_CSRF_TOKEN", token),
            new KeyValuePair<string, string>(hotWater.MeterId,
                hotWater.Value.ToString(CultureInfo.InvariantCulture)), // cold Water
            new KeyValuePair<string, string>(coldWater.MeterId,
                coldWater.Value.ToString(CultureInfo.InvariantCulture)) // hot water
        ]);

        return requestContent;
    }

    private async Task<(string coldWaterMeterId, string hotWaterMeterId)> GetMeterIdsAsync(string email,
        string password, CancellationToken ct = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/counters");

        request.SetAuthOptions(email, password);

        using var response = await _httpClient.SendAsync(request, ct);

        var metersPage = await response.Content.ReadAsStringAsync(ct);

        return new CountersPageParser().GetMeterIds(metersPage);
    }

    private async Task<string> GetCsrfTokenFromCountersPageAsync(string email,
        string password,
        CancellationToken ct)
    {
        using var getCountersPageRequest = new HttpRequestMessage(HttpMethod.Get, "/counters");
        getCountersPageRequest.SetAuthOptions(email, password);

        var getCountersPageResponse = await _httpClient.SendAsync(getCountersPageRequest, ct);

        string countersPage = await getCountersPageResponse.Content.ReadAsStringAsync(ct);

        string csrfToken = new LoginPageParser().GetCsrfToken(countersPage);

        return csrfToken;
    }

    private class Meter
    {
        public string MeterId { get; init; } = null!;

        public decimal Value { get; init; }
    }
}