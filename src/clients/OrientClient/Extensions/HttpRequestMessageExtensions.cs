namespace OrientClient.Extensions;

/// <summary>
/// Contains extension methods for http request message to simplify set auth cookie.
/// </summary>
internal static class HttpRequestMessageExtensions
{
    /// <summary>
    /// Set auth options to request message.
    /// </summary>
    /// <param name="requestMessage">Message to send.</param>
    /// <param name="email">User email.</param>
    /// <param name="password">User password.</param>
    internal static void SetAuthOptions(this HttpRequestMessage requestMessage, string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email can't be empty", nameof(email));
        }
        
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("Password can't be empty", nameof(password));
        }
        
        requestMessage.Options.Set(new HttpRequestOptionsKey<string>("email"), email);
        requestMessage.Options.Set(new HttpRequestOptionsKey<string>("password"), password);
    }

    /// <summary>
    /// Get auth options from request message.
    /// </summary>
    /// <param name="requestMessage">Request message.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    internal static (string? email, string? password)  GetAuthOption(this HttpRequestMessage requestMessage)
    {
        if (!requestMessage.Options.TryGetValue(new HttpRequestOptionsKey<string>("email"), out string? email))
        {
            return (null, null);
        }
        
        if(!requestMessage.Options.TryGetValue(new HttpRequestOptionsKey<string>("password"), out string? password))
        {
            throw new InvalidOperationException("Password was not set in request message.");
        }

        return (email, password);
    }
}