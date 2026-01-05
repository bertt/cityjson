using System.Text.Json;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;

namespace CityJSON;


public static class CityJsonSeqWriter
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = false,                       // Formatting.None
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase, // CamelCaseNamingStrategy
        // DictionaryKeyPolicy = null;               // keep dictionary keys unchanged (matches ProcessDictionaryKeys = false)
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull // NullValueHandling.Ignore
    };

    public static void WriteCityJsonSeq(List<CityJsonDocument> documents, string filePath)
    {
        using var writer = new StreamWriter(filePath);

        foreach (var document in documents)
        {
            var json = JsonSerializer.Serialize(document, Options);
            writer.WriteLine(json);
        }
    }
}
