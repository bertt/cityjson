using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.IO;

namespace CityJSON;

public static class CityJsonSeqWriter
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
