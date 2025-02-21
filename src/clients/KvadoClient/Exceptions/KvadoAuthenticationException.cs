namespace KvadoClient.Exceptions;

/// <summary>
/// The exception that is throw when unable to authorize on site.
/// </summary>
public class KvadoAuthenticationException : KvadoException
{
    /// <summary>
    /// Initialize a new instance of the <see cref="KvadoAuthenticationException"/> class.
    /// </summary>
    /// <param name="message"></param>
    public KvadoAuthenticationException(string message) : base(message)
    {
        
    }
}