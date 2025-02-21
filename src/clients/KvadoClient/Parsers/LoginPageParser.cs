using HtmlAgilityPack;

namespace KvadoClient.Parsers;

public class LoginPageParser
{
    public string GetCsrfToken(string page)
    {
        var document = new HtmlDocument();
        document.LoadHtml(page);

        var token = document.DocumentNode.Descendants("meta")
            .FirstOrDefault(node => node.GetAttributeValue("name", string.Empty) == "csrf-token")
            ?.GetAttributeValue("content", string.Empty);

        return token ?? throw new InvalidOperationException();
    }
}