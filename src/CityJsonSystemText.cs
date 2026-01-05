using System.Text.Json;
using System.Text.Json.Serialization;

namespace CityJSON;

public static class CityJsonSystemTextJson
{
    public static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DictionaryKeyPolicy = null, // keep dictionary keys unchanged
        PropertyNameCaseInsensitive = true, // match Newtonsoft default
        AllowTrailingCommas = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters =
        {
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: true)
        }
    };
}