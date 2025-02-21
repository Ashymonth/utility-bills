using System.Globalization;
using HtmlAgilityPack;
using OrientClient.Exceptions;

namespace OrientClient.Parsers;

internal class DebtParser
{
    /// <summary>
    /// Get client debt from account page.
    /// </summary>
    /// <param name="page">Account page.</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public decimal GetDebt(string page)
    {
        var document = new HtmlDocument();
        document.LoadHtml(page);

        var result = document.DocumentNode.Descendants("a")
            .FirstOrDefault(node => node.HasClass("pay-second"));

        if (result is null)
        {
            throw new OrientException("Unable to parse debt page");
        }

        if (decimal.TryParse(result.InnerText.Trim().Replace(" ", string.Empty).Replace("\"", string.Empty),
                CultureInfo.GetCultureInfo("en-US"), out var debt))
        {
            return debt;
        }

        throw new OrientException(
            $"Unable to convert debt text to decimal value. Debt text: {result.InnerText}");
    }
}