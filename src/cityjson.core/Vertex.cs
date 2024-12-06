using CityJSON.Converters;
using Newtonsoft.Json;

namespace CityJSON
{
    [JsonConverter(typeof(VertexConverter))]
    public class Vertex
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
    }
}
