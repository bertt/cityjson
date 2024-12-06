using CityJSON;
using CityJSON.Extensions;
using Newtonsoft.Json;

public static class TileReader
{
    public static List<string> ReadCityJsonSeqTile(string file)
    {
        var jsonSeq = File.ReadAllText(file);
        var allLines = jsonSeq.Split('\n');
        var firstLine = allLines[0];
        var cityJson = JsonConvert.DeserializeObject<CityJsonDocument>(firstLine);
        var transform = cityJson.Transform;

        var result = GetWkts(allLines,transform);
        return result;
    }


    public static List<string> GetWkts(string[] allLines, Transform transform)
    {
        var wkts = new List<string>();

        for (var i = 1; i < allLines.Length - 1; i++)
        {
            var line = allLines[i];
            var cityObject = JsonConvert.DeserializeObject<CityJsonDocument>(line);

            var wkt = cityObject.ToWkt(transform);
            wkts.Add(wkt);
        }
        return wkts;
    }
}
