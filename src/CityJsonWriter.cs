using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.IO;

namespace CityJSON;

public static class CityJsonWriter
{
    private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
    {
        Formatting = Formatting.Indented,
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

    public static string Write(CityJsonDocument document)
    {
        return JsonConvert.SerializeObject(document, Settings);
    }

    public static void WriteToFile(CityJsonDocument document, string filePath)
    {
        var json = Write(document);
        File.WriteAllText(filePath, json);
    }
}
