using HtmlAgilityPack;

namespace KvadoClient.Parsers;

public class CountersPageParser
{
    private const string MeterIdMarker = "values[";

    public (string coldWaterId, string hotWaterId) GetMeterIds(string countersPage)
    {
        var document = new HtmlDocument();
        document.LoadHtml(countersPage);

        var inputs = document.DocumentNode.Descendants("input")
            .Where(node => node.GetAttributeValue("name", string.Empty).StartsWith(MeterIdMarker))
            .Select(node => node)
            .ToArray();

        var coldWaterId = inputs.FirstOrDefault(node =>
                node.GetAttributeValue("data-name", string.Empty).StartsWith("Холодная вода"))
            ?.GetAttributeValue("name", string.Empty);

        var hotWaterId = inputs.FirstOrDefault(node =>
            node.GetAttributeValue("data-name", string.Empty).StartsWith("Горячая вода"))
            ?.GetAttributeValue("name", string.Empty);

        return (coldWaterId, hotWaterId);
    }
}