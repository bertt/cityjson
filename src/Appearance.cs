using Newtonsoft.Json;
using System.Collections.Generic;

namespace CityJSON;
public class Appearance
{
    public List<Texture> Textures { get; set; }

    [JsonProperty("vertices-texture", Required = Required.Default)]
    public float[][] VerticesTexture { get; set; }
}
