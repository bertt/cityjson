using CityJSON.Convertors;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CityJSON.Geometry
{
    [JsonConverter(typeof(GeometryConverter))]
    public abstract class Geometry
    {
        public GeometryType Type { get; set; }
        public string Lod { get; set; }
    }
}
