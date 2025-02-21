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

    public int GetPreviousHotWaterMeterReadings(string page)
    {
        var previousHotWater = GetTextFromHotWaterMeterReadingsBlock(page, "col-auto block-note ml-auto text-right");
        
        return int.Parse(previousHotWater);
    }

    public DateOnly GetLastDateWhenHotWaterReadingWereSent(string page)
    {
        var lastDateText = GetTextFromHotWaterMeterReadingsBlock(page, "col-auto block-note");

        //we remove "от " symbols
        //example: от 24.01.2024

        var result = lastDateText.AsSpan()[3..];

        return DateOnly.Parse(result);
    }

    private static string GetTextFromHotWaterMeterReadingsBlock(string page, string className)
    {
        var document = new HtmlDocument();
        document.LoadHtml(page);

        var lastDateText = document.DocumentNode.Descendants("div")
            .LastOrDefault(node => node.GetAttributeValue("class", string.Empty) == className)
            ?.InnerText
            ?.Trim();

        if (string.IsNullOrWhiteSpace(lastDateText))
        {
            throw new OrientException("Unable to parse water meter readings block");
        }

        return lastDateText;
    }
}