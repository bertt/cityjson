using System.Text.Json;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CityJSON.Geometry;
public class MultiSolidGeometry : Geometry
{
    public int[][][][][] Boundaries { get; set; }

    [JsonPropertyName("texture")]
    public Dictionary<string, int?[][][][][]> Texture { get; set; }

}
