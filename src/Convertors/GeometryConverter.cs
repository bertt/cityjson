using System;
using System.Collections.Generic;
using CityJSON.Geometry;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CityJSON.Convertors
{
    public class GeometryConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Geometry.Geometry);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);
            var type = jsonObject["type"].ToString().ToLowerInvariant();

            Geometry.Geometry obj = null;

            switch (type)
            {
                case "solid":
                    {
                        var boundaries = jsonObject["boundaries"].ToObject<int[][][][]>();
                        obj = new SolidGeometry() { Type = GeometryType.Solid, Boundaries = boundaries };
                        break;
                    }

                case "compositesurface":
                    {
                        var boundaries = jsonObject["boundaries"].ToObject<int[][][]>();
                        obj = new CompositeSurfaceGeometry() { Type = GeometryType.CompositeSurface, Boundaries = boundaries };
                        break;
                    }

                case "multisurface":
                    {
                        var boundaries = jsonObject["boundaries"].ToObject<int[][][]>();
                        obj = new MultiSurfaceGeometry() { Type = GeometryType.MultiSurface, Boundaries = boundaries };
                        break;
                    }
                case "multisolid":
                    {
                        var boundaries = jsonObject["boundaries"].ToObject<int[][][][][]>();
                        obj = new MultiSolidGeometry() { Type = GeometryType.MultiSolid, Boundaries = boundaries };
                        break;
                    }
                case "compositesolid":
                    {
                        var boundaries = jsonObject["boundaries"].ToObject<int[][][][][]>();
                        obj = new CompositeSolidGeometry() { Type = GeometryType.CompositeSolid, Boundaries = boundaries };
                        break;
                    }

            }

            if (jsonObject["lod"] != null)
            {
                obj.Lod = jsonObject["lod"].ToString();
            }

            if(jsonObject["texture"] != null)
            {
                obj.Texture = jsonObject["texture"].ToObject<Dictionary<string, object>>();
            }

            return obj;
        }

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
