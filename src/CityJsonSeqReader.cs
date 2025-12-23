using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace CityJSON;
public static class CityJsonSeqReader
{
    public static List<CityJsonDocument> ReadCityJsonSeq(string filePath)
    {
        var result = new List<CityJsonDocument>();

        var reader = new StreamReader(filePath);

        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            var cityJsonSecond = JsonSerializer.Deserialize<CityJsonDocument>(line, CityJsonSystemTextJson.Options);
            result.Add(cityJsonSecond);
        }
        return result;
    }
}
