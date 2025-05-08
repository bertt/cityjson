using Newtonsoft.Json;
using System.Collections.Generic;

namespace CityJSON.Geometry
{
    public class CompositeSurfaceGeometry : Geometry
    {
        public int[][][] Boundaries { get; set; }

        [JsonProperty("texture")]
        public Dictionary<string, int?[][][]> Texture { get; set; }
    }
}
