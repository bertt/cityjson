using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CityJSON.Converters
{
    public sealed class VertexConverter : JsonConverter<Vertex>
    {
        public override Vertex Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
                throw new JsonException("Vertex must be a JSON array.");

            reader.Read(); // X
            if (reader.TokenType != JsonTokenType.Number) throw new JsonException("Vertex[0] must be a number.");
            var x = reader.GetDouble();

            reader.Read(); // Y
            if (reader.TokenType != JsonTokenType.Number) throw new JsonException("Vertex[1] must be a number.");
            var y = reader.GetDouble();

            reader.Read(); // Z
            if (reader.TokenType != JsonTokenType.Number) throw new JsonException("Vertex[2] must be a number.");
            var z = reader.GetDouble();

            reader.Read(); // EndArray
            if (reader.TokenType != JsonTokenType.EndArray)
                throw new JsonException("Vertex array must have exactly 3 numbers.");

            return new Vertex { X = x, Y = y, Z = z };
        }

        public override void Write(Utf8JsonWriter writer, Vertex value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            writer.WriteNumberValue(value.X);
            writer.WriteNumberValue(value.Y);
            writer.WriteNumberValue(value.Z);
            writer.WriteEndArray();
        }
    }
}