using System;
using System.Collections.Generic;
using System.Linq;
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
            Dictionary<string, object> textureObject = null;

            Geometry.Geometry obj = null;

            if (jsonObject["texture"] != null)
            {
                textureObject = jsonObject["texture"].ToObject<Dictionary<string, Object>>();
            }

            switch (type)
            {
                case "solid":
                    {
                        var boundaries = jsonObject["boundaries"].ToObject<int[][][][]>();
                        obj = new SolidGeometry() { Type = GeometryType.Solid, Boundaries = boundaries };

                        if (textureObject != null)
                        {
                            ((SolidGeometry)obj).Texture = ReadTextureSolid(jsonObject["texture"].ToObject<Dictionary<string, object>>());
                        }
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
                        if(textureObject != null)
                        {
                            ((MultiSurfaceGeometry)obj).Texture = ReadTexture(jsonObject["texture"].ToObject<Dictionary<string, object>>());
                        }   
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

            return obj;
        }

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }


        private Dictionary<string, int?[][][][]> ReadTextureSolid(Dictionary<string, object> texture)
        {
            return ReadTextureGeneric<int?[][][][]>(texture);
        }

        private Dictionary<string, int?[][][]> ReadTexture(Dictionary<string, object> texture)
        {
            return ReadTextureGeneric<int?[][][]>(texture);
        }

        private Dictionary<string, T> ReadTextureGeneric<T>(Dictionary<string, object> texture)
        {
            return texture.ToDictionary(
                t => t.Key,
                t => ((JObject)t.Value)["values"].ToObject<T>()
            );
        }
    }
}
