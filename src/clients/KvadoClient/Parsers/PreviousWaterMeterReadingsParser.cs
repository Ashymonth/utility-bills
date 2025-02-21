using HtmlAgilityPack;
using KvadoClient.Models;

namespace KvadoClient.Parsers;

public class PreviousWaterMeterReadingsParser
{
    public WaterMeterReadings ParseCurrentWaterMeterReadings(string page)
    {
        var document = new HtmlDocument();
        document.LoadHtml(page);

        var counterRows = GetCounterRows(document);

        var coldWater = GetCurrentWaterMeterReadings(counterRows[0]);
        var hotWaterWater = GetCurrentWaterMeterReadings(counterRows[1]);

        return new WaterMeterReadings { ColdWater = coldWater, HotWater = hotWaterWater };
    }
    
    public WaterMeterReadings ParsePreviousWaterMeterReadings(string page)
    {
        var document = new HtmlDocument();
        document.LoadHtml(page);

        var counterRows = GetCounterRows(document);

        var coldWater = GetPreviousWaterMeterReadings(counterRows[0]);
        var hotWaterWater = GetPreviousWaterMeterReadings(counterRows[1]);

        return new WaterMeterReadings { ColdWater = coldWater, HotWater = hotWaterWater };
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

    private static int GetPreviousWaterMeterReadings(HtmlNode waterMeterReadingsRow)
    {
        var waterMeterReadings = waterMeterReadingsRow.Descendants("td")
            ?.Skip(1)
            .FirstOrDefault()
            ?.Descendants("p")
            ?.FirstOrDefault()
            ?.Descendants("strong")
            ?.FirstOrDefault()
            ?.InnerText;

        if (string.IsNullOrWhiteSpace(waterMeterReadings))
        {
            throw new Exception("Cold water meter reading not found");
        }

        //water meter readings provided in formt like 123 куб.м
        var spaceIndex = waterMeterReadings.IndexOf(' ');

        return int.Parse(waterMeterReadings[..spaceIndex]);
    }
    
    private static int GetCurrentWaterMeterReadings(HtmlNode waterMeterReadingsRow)
    {
        var waterMeterReadings = waterMeterReadingsRow.Descendants("td")
            ?.Skip(2)
            .FirstOrDefault()
            ?.Descendants("div")
            ?.FirstOrDefault()
            ?.Descendants("div")
            ?.FirstOrDefault()
            ?.Descendants("input")
            ?.FirstOrDefault()
            ?.GetAttributeValue("value", string.Empty);

        if (string.IsNullOrWhiteSpace(waterMeterReadings))
        {
            throw new Exception("Cold water meter reading not found");
        }

        return int.Parse(waterMeterReadings);
    }
}