using Newtonsoft.Json;

namespace CityJSON;
public class Texture
{
    [JsonProperty("type", Required = Required.Always)]
    public TextureImageType ImageType { get; set; }
    [JsonProperty(Required = Required.Always)]
    public string Image { get; set; }
    
    [JsonProperty("wrapMode", Required = Required.Default)]
    public TextureWrapMode WrapMode { get; set; }

    public TextureType TextureType { get; set; }

    public float[] BorderColor { get; set; }

}
