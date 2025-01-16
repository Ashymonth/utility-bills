namespace OrientClient.Exceptions;

/// <summary>
/// Exception throws client unable to authenticate in orient site.
/// </summary>
public class OrientAuthenticationException : OrientException
{
    /// <summary>
    /// Create a new instance of the <see cref="OrientAuthenticationException"/>
    /// </summary>
    /// <param name="message"></param>
    public OrientAuthenticationException(string message) : base(message)
    {
    }
}