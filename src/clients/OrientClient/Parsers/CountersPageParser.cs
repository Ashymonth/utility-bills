using HtmlAgilityPack;
using OrientClient.Exceptions;

namespace OrientClient.Parsers;

internal class CountersPageParser
{
    /// <summary>
    /// Get csrf token from counters page.
    /// </summary>
    /// <param name="page">Counters page.</param>
    /// <returns></returns>
    /// <exception cref="OrientException"></exception>
    public string GetCsrfToken(string page)
    {
        var document = new HtmlDocument();
        document.LoadHtml(page);

        var token = document.DocumentNode.Descendants("meta")
            .FirstOrDefault(node => node.GetAttributeValue("name", string.Empty) == "csrf-token")
            ?.GetAttributeValue("content", string.Empty) ?? throw new Exception("csrf-token not found");

        return token;
    }
    
    public DateOnly GetLastDateWhenHotWaterReadingWasSent(string page)
    {
        var document = new HtmlDocument();
        document.LoadHtml(page);

        var lastDateText = document.DocumentNode.Descendants("div")
            .LastOrDefault(node => node.GetAttributeValue("class", string.Empty) == "col-auto block-note")
            ?.InnerText
            ?.Trim();

        if (string.IsNullOrWhiteSpace(lastDateText))
        {
            throw new OrientException("Unable last date when hot water reading was sent");
        }
        
        //example от 24.01.2024

        var result = lastDateText.AsSpan()[3..];

        return DateOnly.Parse(result);
    }
}