using HtmlAgilityPack;
using KvadoClient.Models;

namespace KvadoClient.Parsers;

public class PreviousWaterMeterReadingsParser
{
    public PreviousWaterMeterReadings ParsePreviousWaterMeterReadings(string page)
    {
        var document = new HtmlDocument();
        document.LoadHtml(page);

        var counterRows = document.GetElementbyId("counters")
            .Descendants("tbody")
            .FirstOrDefault()
            ?.Descendants("tr")
            ?.ToArray();

        if (counterRows is null)
        {
            throw new Exception();
        }

        var coldWater = GetWaterMeterReadings(counterRows[0]);
        var hotWaterWater = GetWaterMeterReadings(counterRows[1]);

        return new PreviousWaterMeterReadings { ColdWater = coldWater, HotWater = hotWaterWater };
    }

    private static double GetWaterMeterReadings(HtmlNode waterMeterReadingsRow)
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

        return Convert.ToDouble(waterMeterReadings[..spaceIndex]);
    }
}