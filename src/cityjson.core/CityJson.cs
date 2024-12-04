using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CityJSON
{
    public class CityJson
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("version")]
        public string Version { get; set; }

        [JsonPropertyName("transform")]
        public Transform Transform { get; set; }

        [JsonPropertyName("CityObjects")]
        public Dictionary<string, CityObject> CityObjects { get; set; }

        [JsonPropertyName("vertices")]
        public List<List<double>> Vertices { get; set; }
    }
}
