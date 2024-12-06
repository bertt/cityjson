using System;
using CityJSON.Geometry;
using Newtonsoft.Json;

namespace CityJSON.Converters
{
    internal class GeometryTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(GeometryType) || t == typeof(GeometryType?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value?.ToLowerInvariant())
            {
                case "compositesolid":
                    return GeometryType.CompositeSolid;
                case "compositesurface":
                    return GeometryType.CompositeSurface;
                case "geometryinstance":
                    return GeometryType.GeometryInstance;
                case "multiLinestring":
                    return GeometryType.MultiLineString;
                case "multipoint":
                    return GeometryType.MultiPoint;
                case "multisolid":
                    return GeometryType.MultiSolid;
                case "multisurface":
                    return GeometryType.MultiSurface;
                case "solid":
                    return GeometryType.Solid;
            }
            throw new Exception("Cannot unmarshal type GeometryType");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (GeometryType)untypedValue;
            switch (value)
            {
                case GeometryType.CompositeSolid:
                    serializer.Serialize(writer, "CompositeSolid");
                    return;
                case GeometryType.CompositeSurface:
                    serializer.Serialize(writer, "CompositeSurface");
                    return;
                case GeometryType.GeometryInstance:
                    serializer.Serialize(writer, "GeometryInstance");
                    return;
                case GeometryType.MultiLineString:
                    serializer.Serialize(writer, "MultiLineString");
                    return;
                case GeometryType.MultiPoint:
                    serializer.Serialize(writer, "MultiPoint");
                    return;
                case GeometryType.MultiSolid:
                    serializer.Serialize(writer, "MultiSolid");
                    return;
                case GeometryType.MultiSurface:
                    serializer.Serialize(writer, "MultiSurface");
                    return;
                case GeometryType.Solid:
                    serializer.Serialize(writer, "Solid");
                    return;
            }
            throw new Exception("Cannot marshal type GeometryType");
        }

        public static readonly GeometryTypeConverter Singleton = new GeometryTypeConverter();
    }
}