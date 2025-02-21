using HtmlAgilityPack;
using OrientClient.Exceptions;

namespace OrientClient.Parsers;

internal class AccountIdParser
{
    /// <summary>
    /// Get account id.
    /// </summary>
    /// <param name="page">Account page.</param>
    /// <returns></returns>
    /// <exception cref="OrientAuthenticationException"></exception>
    public string GetAccountId(string page)
    {
        var document = new HtmlDocument();
        document.LoadHtml(page);

        string? result = document.DocumentNode
            .Descendants("a")
            .FirstOrDefault(node => node.HasClass("pay-second"))
            ?.GetAttributeValue("href", string.Empty);

        if (result is null)
        {
            throw new OrientAuthenticationException("Unable to parse account id. Check email or password");
        }

        // input string in format https://lk.hppp.ru/account/accountId/payment
        
        int lastSlashIndex = result.LastIndexOf("/", StringComparison.Ordinal) - 1; // find last slash in url

        int previousSlashIndex = 0;
        
        for (int i = lastSlashIndex; i < result.Length; i--)
        {
            if (result[i] != '/') // wait for slash before account id
            {
                continue;
            }
            
            previousSlashIndex = i + 1; // take string without slash
            break;
        }


        return result.Substring(previousSlashIndex, result.Length - lastSlashIndex);
    }
}