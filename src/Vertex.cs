using CityJSON.Converters;
using Newtonsoft.Json;
using System.Numerics;

namespace CityJSON
{
    [JsonConverter(typeof(VertexConverter))]
    public class Vertex
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public Vector3 ToVector3()
        {
            return new Vector3((float)X, (float)Y, (float)Z);
        }
    }
}
