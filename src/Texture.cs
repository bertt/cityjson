using System.Text.Json;
using System.Text.Json.Serialization;

namespace CityJSON;
public class Texture
{
    [JsonPropertyName("type")]
    public TextureImageType ImageType { get; set; }
    
    [JsonRequired]
    public string Image { get; set; }
    
    [JsonPropertyName("wrapMode")]
    public TextureWrapMode WrapMode { get; set; }

    public TextureType TextureType { get; set; }

    public float[] BorderColor { get; set; }

}
