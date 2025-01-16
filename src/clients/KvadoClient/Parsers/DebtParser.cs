using System.Globalization;
using HtmlAgilityPack;

namespace KvadoClient.Parsers;

public class DebtParser
{
    public decimal? ParseDebt(string page)
    {
        var document = new HtmlDocument();
        document.LoadHtml(page);

        var result = document.DocumentNode
            .Descendants("a")
            ?.FirstOrDefault(node => node.GetAttributeValue("href", string.Empty) == "/accruals")
            ?.Descendants("strong")
            ?.FirstOrDefault()
            ?.InnerHtml;

        if (result?[0] == '0') // no debt
        {
            return null;
        }

        var span = result.AsSpan();
        var indexOfNumberEnd = span.IndexOf('&');

        return decimal.Parse(span[..(indexOfNumberEnd - 1)].ToString(), CultureInfo.GetCultureInfo("ru-Ru"));
    }
}