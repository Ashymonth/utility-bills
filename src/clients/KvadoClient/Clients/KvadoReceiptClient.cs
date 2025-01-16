// using KvadoClient.Extensions;
//
// namespace KvadoClient.Clients;
//
// public class KvadoReceiptClient
// {
//     private readonly HttpClient _httpClient;
//
//     public KvadoReceiptClient(HttpClient httpClient)
//     {
//         _httpClient = httpClient;
//     }
//     
//     public async Task<Stream> GetReceiptAsync(string email, string password, DateOnly month,
//         CancellationToken ct = default)
//     {
//         using var request = new HttpRequestMessage(HttpMethod.Get, "/receipt/lkReceiptByMonth?accounts_id=3308106&month%5B0%5D=9&users_id=314739&year=2024&organizations_id=5494&token={3}");
//         request.SetAuthOptions(email, password);
//     }
// }