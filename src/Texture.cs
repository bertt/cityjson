using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CityJSON;
public class Texture
{
    [JsonConverter(typeof(StringEnumConverter))]
    [JsonProperty("type", Required = Required.Always)]
    public TextureImageType ImageType { get; set; }
    [JsonProperty(Required = Required.Always)]
    public string Image { get; set; }
    
    [JsonConverter(typeof(StringEnumConverter))]
    [JsonProperty("wrapMode", Required = Required.Default)]
    public TextureWrapMode WrapMode { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    public TextureType TextureType { get; set; }

    public float[] BorderColor { get; set; }

}
