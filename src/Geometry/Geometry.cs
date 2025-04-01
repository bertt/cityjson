using CityJSON.Convertors;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace CityJSON.Geometry
{
    [JsonConverter(typeof(GeometryConverter))]
    public abstract class Geometry
    {
        public GeometryType Type { get; set; }
        public string Lod { get; set; }

        [JsonProperty("texture")]
        public Dictionary<string, int?[][][]> Texture { get; set; }
    }
}
