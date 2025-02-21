namespace OrientClient.Exceptions;

/// <summary>
/// Base exception for custom exceptions.
/// </summary>
public class OrientException : Exception
{
    /// <summary>
    /// Create a new instance of the <see cref="OrientException"/>
    /// </summary>
    /// <param name="message"></param>
    public OrientException(string message) : base(message)
    {
        
    }
}