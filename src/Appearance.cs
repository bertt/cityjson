using System.Text.Json;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CityJSON;
public class Appearance
{
    public List<Texture> Textures { get; set; }

    [JsonPropertyName("vertices-texture")]
    public float[][] VerticesTexture { get; set; }
}
