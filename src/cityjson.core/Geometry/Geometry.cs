using CityJSON.Converters;
using CityJSON.Convertors;
using Newtonsoft.Json;

namespace CityJSON.Geometry
{
    [JsonConverter(typeof(GeometryConverter))]
    public abstract class Geometry
    {
        [JsonConverter(typeof(GeometryTypeConverter))]
        public GeometryType Type { get; set; }
        public string Lod { get; set; }
    }
}
