using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.IO;

namespace CityJSON;
public static class CityJsonSeqReader
{
    private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
    {
        Formatting = Formatting.None,
        ContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new CamelCaseNamingStrategy
            {
                ProcessDictionaryKeys = false,
                OverrideSpecifiedNames = false
            }
        },
        NullValueHandling = NullValueHandling.Ignore
    };

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

    public static void WriteCityJsonSeq(List<CityJsonDocument> documents, string filePath)
    {
        using (var writer = new StreamWriter(filePath))
        {
            foreach (var document in documents)
            {
                var json = JsonConvert.SerializeObject(document, Settings);
                writer.WriteLine(json);
            }
        }
    }
}
