using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CityJSON
{
    public class Geometry
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        //[JsonPropertyName("boundaries")]
        // public List<List<List<List<int>>>> Boundaries { get; set; }

        [JsonPropertyName("semantics")]
        public Semantics Semantics { get; set; }

        [JsonPropertyName("material")]
        public Dictionary<string, Material> Material { get; set; }

        [JsonPropertyName("lod")]
        public int Lod { get; set; }
    }
}
