namespace OrientClient.Exceptions;

/// <summary>
/// Exception throws when counters data edited in wrong day.
/// </summary>
public class OrientDateException : OrientException
{
    /// <summary>
    /// Create a new instance of the <see cref="OrientAuthenticationException"/>
    /// </summary>
    /// <param name="message"></param>
    public OrientDateException(string message) : base(message)
    {
    }
}