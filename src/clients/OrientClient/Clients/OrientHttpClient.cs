using System.Text.Json;
using OrientClient.Exceptions;
using OrientClient.Extensions;
using OrientClient.Models;
using OrientClient.Parsers;

namespace OrientClient.Clients;

/// <summary>
/// Client to connect to https://lk.hppp.ru/
/// </summary>
public interface IOrientClient
{
    /// <summary>
    /// Remove already send counters data. 
    /// </summary>
    /// <param name="email">User email.</param>
    /// <param name="password">User password.</param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task DeleteCountersDataAsync(string email, string password, CancellationToken ct = default);

    /// <summary>
    /// Send water readings.
    /// </summary>
    /// <param name="email">User email.</param>
    /// <param name="password">User password.</param>
    /// <param name="hotWater">Hot water value.</param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task SendWaterReadingsAsync(string email,
        string password,
        int hotWater,
        CancellationToken ct = default);

    /// <summary>
    /// Get user debt.
    /// </summary>
    /// <param name="email">User email to log in.</param>
    /// <param name="password">Use the password to log in.</param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<decimal> GetDebtAsync(string email, string password, CancellationToken ct = default);

    /// <summary>
    /// Check that site is alive.
    /// </summary>
    /// <param name="email">User email to log in.</param>
    /// <param name="password">Use the password to log in.</param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<bool> IsSiteAvailable(string email, string password, CancellationToken ct = default);

    /// <summary>
    /// Get prev sent hot water meter readings.
    /// </summary>
    /// <param name="email">User email to log in.</param>
    /// <param name="password">Use the password to log in.</param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<int> GetPreviousWaterMeterReadingAsync(string email, string password, CancellationToken ct =
        default);

    /// <summary>
    /// Get the last date when hot water readings were updated.
    /// </summary>
    /// <param name="email">User email to log in.</param>
    /// <param name="password">Use the password to log in.</param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<DateOnly> LastDateWhenHotWaterReadingWereSentAsync(string email, string password,
        CancellationToken ct = default);
}

/// <summary>
/// <see cref="IOrientClient"/>
/// </summary>
public class OrientHttpClient : IOrientClient
{
    private const string AccountUrl = "/account/add";
    private const string CountersUrlTemplate = "/account/{0}/counters";
    private const string RemoveCountersUrlTemplate = "/account/{0}/counters/remove";

    private const int StartEditCountersDate = 20;
    private const int EndEditCountersDate = 25;

    private readonly HttpClient _httpClient;
    private readonly IUserProvider _userProvider;

    /// <summary>
    /// Create a new instance of the <see cref="OrientHttpClient"/>
    /// </summary>
    /// <param name="httpClient"></param>
    /// <param name="userProvider"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public OrientHttpClient(HttpClient httpClient, IUserProvider userProvider)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _userProvider = userProvider ?? throw new ArgumentNullException(nameof(userProvider));
    }

    /// <inheritdoc />
    public async Task DeleteCountersDataAsync(string email, string password, CancellationToken ct = default)
    {
        if (DateTime.Now.Date.Day is < StartEditCountersDate or > EndEditCountersDate)
        {
            throw new OrientDateException("Counters can be edit only from 20 to 25 days");
        }

        var (csrfToken, cookie) = await GetCsrfTokenFromCountersPageAsync(email, password, ct);

        using var sendCountersRequest =
            new HttpRequestMessage(HttpMethod.Post, string.Format(RemoveCountersUrlTemplate, csrfToken));
        sendCountersRequest.Headers.TryAddWithoutValidation("Cookie", cookie);

        sendCountersRequest.SetAuthOptions(email, password);

        sendCountersRequest.Content = CreateRemoveWaterReadingsContent(csrfToken);

        using var sendCountersResponse = await _httpClient.SendAsync(sendCountersRequest, ct);

        sendCountersResponse.EnsureSuccessStatusCode();
    }

    /// <inheritdoc />
    public async Task SendWaterReadingsAsync(string email,
        string password,
        int hotWater,
        CancellationToken ct = default)
    {
        if (DateTime.Now.Date.Day is < StartEditCountersDate or > EndEditCountersDate)
        {
            throw new OrientDateException("Counters can be edit only from 20 to 25 days");
        }

        if (hotWater < 0)
        {
            throw new OrientException("Hot water can't be less that 0");
        }

        string accountId = await _userProvider.GetAccountIdAsync(email, password, ct);

        var (csrfToken, cookie) = await GetCsrfTokenFromCountersPageAsync(email, password, ct);

        using var sendCountersRequest =
            new HttpRequestMessage(HttpMethod.Post, string.Format(CountersUrlTemplate, accountId));

        sendCountersRequest.Headers.TryAddWithoutValidation("Cookie", cookie);

        sendCountersRequest.Content = CreateSendWaterReadingsContent(csrfToken, hotWater);

        using var sendCountersResponse = await _httpClient.SendAsync(sendCountersRequest, ct);

        sendCountersResponse.EnsureSuccessStatusCode();
    }

    /// <inheritdoc />
    public async Task<decimal> GetDebtAsync(string email, string password, CancellationToken ct = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, AccountUrl);
        request.SetAuthOptions(email, password);

        using var response = await _httpClient.SendAsync(request, ct);

        string page = await response.Content.ReadAsStringAsync(ct);

        decimal debt = new DebtParser().GetDebt(page);

        return debt;
    }

    /// <inheritdoc />
    public async Task<bool> IsSiteAvailable(string email, string password, CancellationToken ct = default)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, "/");
            request.SetAuthOptions(email, password);

            using var response = await _httpClient.SendAsync(request, ct);

            response.EnsureSuccessStatusCode();
        }
        catch (Exception)
        {
            return false;
        }

        return false;
    }

    /// <inheritdoc />
    public async Task<int> GetPreviousWaterMeterReadingAsync(string email, string password, CancellationToken ct =
        default)
    {
        string accountId = await _userProvider.GetAccountIdAsync(email, password, ct);

        using var request = new HttpRequestMessage(HttpMethod.Get, string.Format(CountersUrlTemplate, accountId));
        request.SetAuthOptions(email, password);

        using var response = await _httpClient.SendAsync(request, ct);

        var page = await response.Content.ReadAsStringAsync(ct);

        return new CountersPageParser().GetPreviousHotWaterMeterReadings(page);
    }

    /// <inheritdoc />
    public async Task<DateOnly> LastDateWhenHotWaterReadingWereSentAsync(string email, string password,
        CancellationToken ct =
            default)
    {
        string accountId = await _userProvider.GetAccountIdAsync(email, password, ct);

        using var request = new HttpRequestMessage(HttpMethod.Get, string.Format(CountersUrlTemplate, accountId));
        request.SetAuthOptions(email, password);

        using var response = await _httpClient.SendAsync(request, ct);

        var page = await response.Content.ReadAsStringAsync(ct);

        return new CountersPageParser().GetLastDateWhenHotWaterReadingWereSent(page);
    }

    private static FormUrlEncodedContent CreateSendWaterReadingsContent(string token, int hotWater)
    {
        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("_token", token),
            new KeyValuePair<string, string>("counters[15006_0][value]", hotWater.ToString()),
            new KeyValuePair<string, string>("counters[15006_0][rowId]", "15006"),
            new KeyValuePair<string, string>("counters[15006_0][tarif]", "0"),
            new KeyValuePair<string, string>("counters[14243_0][value]", string.Empty),
            new KeyValuePair<string, string>("counters[14243_0][rowId]", "14243"),
            new KeyValuePair<string, string>("counters[14243_0][tarif]", "0"),
        });

        return content;
    }

    private static FormUrlEncodedContent CreateRemoveWaterReadingsContent(string token)
    {
        var countersParams = new RemoveCountersModel[]
        {
            new()
            {
                Id = 15006,
                Key = "counters[15006_0][value]",
                Traiff = 0,
                Value = 7751747
            }
        };

        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("_token", token),
            new KeyValuePair<string, string>("counter_params", JsonSerializer.Serialize(countersParams))
        });

        return content;
    }

    private async Task<(string tokem, string[] cookie)> GetCsrfTokenFromCountersPageAsync(string email,
        string password,
        CancellationToken ct)
    {
        string accountId = await _userProvider.GetAccountIdAsync(email, password, ct);

        using var request = new HttpRequestMessage(HttpMethod.Get, string.Format(CountersUrlTemplate, accountId));
        request.SetAuthOptions(email, password);

        using var accountPageResponse = await _httpClient.SendAsync(request, ct);

        accountPageResponse.EnsureSuccessStatusCode();

        string countersPage = await accountPageResponse.Content.ReadAsStringAsync(ct);

        var token = new CountersPageParser().GetCsrfToken(countersPage);

        var cookie = accountPageResponse.Headers.GetValues("Set-Cookie")
            .Select(s => s.Substring(0, s.IndexOf(';') + 1)).ToArray();

        return (token, [cookie[0] + cookie[1]]);
    }
}