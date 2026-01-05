using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CityJSON;

public static class CityJsonWriter
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,                         // Formatting.Indented
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        // DictionaryKeyPolicy = null;                // keep dictionary keys unchanged
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public static string Write(CityJsonDocument document)
        => JsonSerializer.Serialize(document, Options);

    public static void WriteToFile(CityJsonDocument document, string filePath)
        => File.WriteAllText(filePath, Write(document));
}