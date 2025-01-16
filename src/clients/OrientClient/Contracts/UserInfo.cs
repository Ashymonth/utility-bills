namespace OrientClient.Contracts;

/// <summary>
/// Response after login.
/// </summary>
public class UserInfo
{
    /// <summary>
    /// User id on the site.
    /// </summary>
    public string AccountId { get; init; } = null!;
    
    /// <summary>
    /// User authorization cookie.
    /// </summary>
    public string[] Cookie { get; init; } = null!;
}