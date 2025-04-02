using CityJSON;
using CityJSON.Extensions;
using NetTopologySuite.Features;
using Newtonsoft.Json;

public static class TileReader
{
    public static List<Feature> ReadCityJsonSeqTile(string file, string? lod=null)
    {
        var jsonSeq = File.ReadAllText(file);
        var allLines = jsonSeq.Split('\n');
        var firstLine = allLines[0];
        var cityJson = JsonConvert.DeserializeObject<CityJsonDocument>(firstLine);
        var transform = cityJson!.Transform;

        var result = GetFeatures(allLines,transform, lod);
        return result;
    }


    public static List<Feature> GetFeatures(string[] allLines, Transform transform, string? lod = null)
    {
        var features = new List<Feature>();

        for (var i = 1; i < allLines.Length - 1; i++)
        {
            var line = allLines[i];
            var cityObject = JsonConvert.DeserializeObject<CityJsonDocument>(line);

            var feature = cityObject.ToFeature(transform, lod);
            features.Add(feature);
        }
        return features;
    }
}
