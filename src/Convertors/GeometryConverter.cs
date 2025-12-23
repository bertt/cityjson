using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using CityJSON.Geometry;

namespace CityJSON.Convertors
{
    public sealed class GeometryConverter : JsonConverter<Geometry.Geometry>
    {
        public override Geometry.Geometry Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            var root = doc.RootElement;

            if (!root.TryGetProperty("type", out var typeEl) || typeEl.ValueKind != JsonValueKind.String)
                throw new JsonException("Geometry missing 'type'.");

            var type = typeEl.GetString()!.ToLowerInvariant();

            Geometry.Geometry obj = type switch
            {
                "solid" => ReadSolid(root, options),
                "compositesurface" => new CompositeSurfaceGeometry
                {
                    Type = GeometryType.CompositeSurface,
                    Boundaries = ReadRequired(root, "boundaries").Deserialize<int[][][]>(options)
                                 ?? throw new JsonException("Invalid 'boundaries' for CompositeSurface.")
                },
                "multisurface" => ReadMultiSurface(root, options),
                "multisolid" => new MultiSolidGeometry
                {
                    Type = GeometryType.MultiSolid,
                    Boundaries = ReadRequired(root, "boundaries").Deserialize<int[][][][][]>(options)
                                 ?? throw new JsonException("Invalid 'boundaries' for MultiSolid.")
                },
                "compositesolid" => new CompositeSolidGeometry
                {
                    Type = GeometryType.CompositeSolid,
                    Boundaries = ReadRequired(root, "boundaries").Deserialize<int[][][][][]>(options)
                                 ?? throw new JsonException("Invalid 'boundaries' for CompositeSolid.")
                },
                _ => null
            };

            if (root.TryGetProperty("lod", out var lodEl) && lodEl.ValueKind != JsonValueKind.Null)
            {
                obj.Lod = lodEl.ValueKind switch
                {
                    JsonValueKind.String => lodEl.GetString(),
                    JsonValueKind.Number => lodEl.GetRawText(), // "2"
                    _ => throw new JsonException("'lod' must be string or number.")
                };
            }

            return obj;
        }

        public override void Write(Utf8JsonWriter writer, Geometry.Geometry value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteString("type", value.Type.ToString());

            if (!string.IsNullOrEmpty(value.Lod))
                writer.WriteString("lod", value.Lod);

            writer.WritePropertyName("boundaries");

            switch (value.Type)
            {
                case GeometryType.Solid:
                {
                    var solid = (SolidGeometry)value;
                    JsonSerializer.Serialize(writer, solid.Boundaries, options);

                    if (solid.Texture != null)
                    {
                        writer.WritePropertyName("texture");
                        WriteTexture(writer, solid.Texture, options);
                    }
                    break;
                }
                case GeometryType.CompositeSurface:
                {
                    var g = (CompositeSurfaceGeometry)value;
                    JsonSerializer.Serialize(writer, g.Boundaries, options);

                    if (g.Texture != null)
                    {
                        writer.WritePropertyName("texture");
                        WriteTexture(writer, g.Texture, options);
                    }
                    break;
                }
                case GeometryType.MultiSurface:
                {
                    var g = (MultiSurfaceGeometry)value;
                    JsonSerializer.Serialize(writer, g.Boundaries, options);

                    if (g.Texture != null)
                    {
                        writer.WritePropertyName("texture");
                        WriteTexture(writer, g.Texture, options);
                    }
                    break;
                }
                case GeometryType.MultiSolid:
                {
                    var g = (MultiSolidGeometry)value;
                    JsonSerializer.Serialize(writer, g.Boundaries, options);

                    if (g.Texture != null)
                    {
                        writer.WritePropertyName("texture");
                        WriteTexture(writer, g.Texture, options);
                    }
                    break;
                }
                case GeometryType.CompositeSolid:
                {
                    var g = (CompositeSolidGeometry)value;
                    JsonSerializer.Serialize(writer, g.Boundaries, options);

                    if (g.Texture != null)
                    {
                        writer.WritePropertyName("texture");
                        WriteTexture(writer, g.Texture, options);
                    }
                    break;
                }
                default:
                    throw new JsonException($"Unsupported geometry type '{value.Type}'.");
            }

            writer.WriteEndObject();
        }

        private static SolidGeometry ReadSolid(JsonElement root, JsonSerializerOptions options)
        {
            var solid = new SolidGeometry
            {
                Type = GeometryType.Solid,
                Boundaries = ReadRequired(root, "boundaries").Deserialize<int[][][][]>(options)
                             ?? throw new JsonException("Invalid 'boundaries' for Solid.")
            };

            if (TryGetNonNull(root, "texture", out var texEl))
                solid.Texture = ReadTextureGeneric<int?[][][][]>(texEl, options);

            return solid;
        }

        private static MultiSurfaceGeometry ReadMultiSurface(JsonElement root, JsonSerializerOptions options)
        {
            var ms = new MultiSurfaceGeometry
            {
                Type = GeometryType.MultiSurface,
                Boundaries = ReadRequired(root, "boundaries").Deserialize<int[][][]>(options)
                             ?? throw new JsonException("Invalid 'boundaries' for MultiSurface.")
            };

            if (TryGetNonNull(root, "texture", out var texEl))
                ms.Texture = ReadTextureGeneric<int?[][][]>(texEl, options);

            return ms;
        }

        private static JsonElement ReadRequired(JsonElement root, string name)
        {
            if (!root.TryGetProperty(name, out var el))
                throw new JsonException($"Geometry missing '{name}'.");
            return el;
        }

        private static bool TryGetNonNull(JsonElement root, string name, out JsonElement el)
        {
            if (root.TryGetProperty(name, out el) && el.ValueKind != JsonValueKind.Null)
                return true;
            el = default;
            return false;
        }

        private static Dictionary<string, T> ReadTextureGeneric<T>(JsonElement textureEl, JsonSerializerOptions options)
        {
            if (textureEl.ValueKind != JsonValueKind.Object)
                throw new JsonException("'texture' must be an object.");

            var dict = new Dictionary<string, T>(StringComparer.Ordinal);

            foreach (var prop in textureEl.EnumerateObject())
            {
                if (!prop.Value.TryGetProperty("values", out var valuesEl))
                    throw new JsonException("Texture entry missing 'values'.");

                var value = valuesEl.Deserialize<T>(options);
                if (value is null)
                    throw new JsonException("Invalid texture 'values'.");

                dict[prop.Name] = value;
            }

            return dict;
        }

        private static void WriteTexture<T>(Utf8JsonWriter writer, IDictionary<string, T> texture, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            foreach (var kvp in texture)
            {
                writer.WritePropertyName(kvp.Key);
                writer.WriteStartObject();
                writer.WritePropertyName("values");
                JsonSerializer.Serialize(writer, kvp.Value, options);
                writer.WriteEndObject();
            }

            writer.WriteEndObject();
        }
    }
}
