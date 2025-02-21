using HtmlAgilityPack;
using OrientClient.Exceptions;

namespace OrientClient.Parsers;

internal class LoginPageParser
{
    /// <summary>
    /// Get csrf token from login page.
    /// </summary>
    /// <param name="page">Login page.</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public string GetCsrfToken(string page)
    {
        var document = new HtmlDocument();
        document.LoadHtml(page);

        string? result = document.GetElementbyId("authorization_form")
            ?.Descendants("input")
            .FirstOrDefault()
            ?.GetAttributeValue("value", string.Empty);

        if (result is null)
        {
            throw new OrientException("Unable to parse token from login page");
        }

        return result;
    }
}