using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CityJSON.Converters
{
    public class VertexConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(Vertex));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var geometry = JArray.Load(reader);
            var vertices = geometry.Values<double>().ToList();
            return new Vertex
            {
                X = vertices[0],
                Y = vertices[1],
                Z = vertices[2]
            };
        }

        public override bool CanWrite => true;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var vertex = (Vertex)value;
            serializer.Serialize(writer, new[] { vertex.X, vertex.Y, vertex.Z });
        }
    }
}