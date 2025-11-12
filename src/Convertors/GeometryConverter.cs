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

        public override bool CanWrite => true;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var geometry = (Geometry.Geometry)value;
            
            writer.WriteStartObject();
            
            // Write type
            writer.WritePropertyName("type");
            writer.WriteValue(geometry.Type.ToString());
            
            // Write lod if present
            if (!string.IsNullOrEmpty(geometry.Lod))
            {
                writer.WritePropertyName("lod");
                writer.WriteValue(geometry.Lod);
            }
            
            // Write boundaries based on type
            writer.WritePropertyName("boundaries");
            switch (geometry.Type)
            {
                case GeometryType.Solid:
                    var solid = (SolidGeometry)geometry;
                    serializer.Serialize(writer, solid.Boundaries);
                    if (solid.Texture != null)
                    {
                        writer.WritePropertyName("texture");
                        WriteTexture(writer, serializer, solid.Texture);
                    }
                    break;
                    
                case GeometryType.CompositeSurface:
                    var compositeSurface = (CompositeSurfaceGeometry)geometry;
                    serializer.Serialize(writer, compositeSurface.Boundaries);
                    if (compositeSurface.Texture != null)
                    {
                        writer.WritePropertyName("texture");
                        WriteTexture(writer, serializer, compositeSurface.Texture);
                    }
                    break;
                    
                case GeometryType.MultiSurface:
                    var multiSurface = (MultiSurfaceGeometry)geometry;
                    serializer.Serialize(writer, multiSurface.Boundaries);
                    if (multiSurface.Texture != null)
                    {
                        writer.WritePropertyName("texture");
                        WriteTexture(writer, serializer, multiSurface.Texture);
                    }
                    break;
                    
                case GeometryType.MultiSolid:
                    var multiSolid = (MultiSolidGeometry)geometry;
                    serializer.Serialize(writer, multiSolid.Boundaries);
                    if (multiSolid.Texture != null)
                    {
                        writer.WritePropertyName("texture");
                        WriteTexture(writer, serializer, multiSolid.Texture);
                    }
                    break;
                    
                case GeometryType.CompositeSolid:
                    var compositeSolid = (CompositeSolidGeometry)geometry;
                    serializer.Serialize(writer, compositeSolid.Boundaries);
                    if (compositeSolid.Texture != null)
                    {
                        writer.WritePropertyName("texture");
                        WriteTexture(writer, serializer, compositeSolid.Texture);
                    }
                    break;
            }
            
            writer.WriteEndObject();
        }
        
        private void WriteTexture<T>(JsonWriter writer, JsonSerializer serializer, Dictionary<string, T> texture)
        {
            writer.WriteStartObject();
            foreach (var kvp in texture)
            {
                writer.WritePropertyName(kvp.Key);
                writer.WriteStartObject();
                writer.WritePropertyName("values");
                serializer.Serialize(writer, kvp.Value);
                writer.WriteEndObject();
            }
            writer.WriteEndObject();
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
