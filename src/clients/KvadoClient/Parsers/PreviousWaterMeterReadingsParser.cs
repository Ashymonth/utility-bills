using HtmlAgilityPack;
using KvadoClient.Models;

namespace KvadoClient.Parsers;

public class PreviousMeterReadingsParser
{
    public MeterReadings? ParseCurrentMeterReadings(string page)
    {
        var document = new HtmlDocument();
        document.LoadHtml(page);

        var counterRows = GetCounterRows(document);

        var coldWater = GetCurrentMeterReadings(counterRows[0]);
        var hotWaterWater = GetCurrentMeterReadings(counterRows[1]);

        if (coldWater is null || hotWaterWater is null)
        {
            return null;
        }

        return new MeterReadings { ColdWater = coldWater.Value, HotWater = hotWaterWater.Value };
    }
    
    public MeterReadings ParsePreviousMeterReadings(string page)
    {
        var document = new HtmlDocument();
        document.LoadHtml(page);

        var counterRows = GetCounterRows(document);

        var coldWater = GetPreviousMeterReadings(counterRows[0]);
        var hotWaterWater = GetPreviousMeterReadings(counterRows[1]);

        return new MeterReadings { ColdWater = coldWater, HotWater = hotWaterWater };
    }

    private static HtmlNode[] GetCounterRows(HtmlDocument document)
    {
        var counterRows = document.GetElementbyId("counters")
            .Descendants("tbody")
            .FirstOrDefault()
            ?.Descendants("tr")
            ?.ToArray();

        if (counterRows is null)
        {
            throw new Exception();
        }

        return counterRows;
    }

    private static int GetPreviousMeterReadings(HtmlNode MeterReadingsRow)
    {
        var MeterReadings = MeterReadingsRow.Descendants("td")
            ?.Skip(1)
            .FirstOrDefault()
            ?.Descendants("p")
            ?.FirstOrDefault()
            ?.Descendants("strong")
            ?.FirstOrDefault()
            ?.InnerText;

        if (string.IsNullOrWhiteSpace(MeterReadings))
        {
            throw new Exception("Cold water meter reading not found");
        }

        //water meter readings provided in formt like 123 куб.м
        var spaceIndex = MeterReadings.IndexOf(' ');

        return int.Parse(MeterReadings[..spaceIndex]);
    }
    
    private static int? GetCurrentMeterReadings(HtmlNode MeterReadingsRow)
    {
        var MeterReadings = MeterReadingsRow.Descendants("td")
            ?.Skip(2)
            .FirstOrDefault()
            ?.Descendants("div")
            ?.FirstOrDefault()
            ?.Descendants("div")
            ?.FirstOrDefault()
            ?.Descendants("input")
            ?.FirstOrDefault()
            ?.GetAttributeValue("value", string.Empty);

        if (string.IsNullOrWhiteSpace(MeterReadings))
        {
            return null;
        }

        return int.Parse(MeterReadings);
    }
}