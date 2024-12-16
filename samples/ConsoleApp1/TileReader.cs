using CityJSON;
using CityJSON.Extensions;
using Newtonsoft.Json;

public static class TileReader
{
    public static List<string> ReadCityJsonSeqTile(string file, string lod=null)
    {
        var jsonSeq = File.ReadAllText(file);
        var allLines = jsonSeq.Split('\n');
        var firstLine = allLines[0];
        var cityJson = JsonConvert.DeserializeObject<CityJsonDocument>(firstLine);
        var transform = cityJson.Transform;

        var result = GetWkts(allLines,transform, lod);
        return result;
    }


    public static List<string> GetWkts(string[] allLines, Transform transform, string lod = null)
    {
        var wkts = new List<string>();

        for (var i = 1; i < allLines.Length - 1; i++)
        {
            var line = allLines[i];
            var cityObject = JsonConvert.DeserializeObject<CityJsonDocument>(line);

            var wkt = cityObject.ToWkt(transform, lod);
            wkts.Add(wkt);
        }
        return wkts;
    }
}
