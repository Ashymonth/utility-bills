namespace KvadoClient.Exceptions;

/// <summary>
/// Represent custom error that throw on custom application error.
/// </summary>
public class KvadoException : Exception
{
    /// <summary>
    /// Initialize a new instance of the <see cref="KvadoException"/> class.
    /// </summary>
    /// <param name="message"></param>
    public KvadoException(string message)
    {
        
    }
}