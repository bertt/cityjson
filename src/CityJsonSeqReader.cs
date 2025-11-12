using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

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
            var cityJsonSecond = JsonConvert.DeserializeObject<CityJsonDocument>(line);
            result.Add(cityJsonSecond);
        }
        return result;
    }
}
