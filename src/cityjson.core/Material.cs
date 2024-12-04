using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CityJSON
{
    public class Material
    {
        [JsonPropertyName("values")]
        public List<List<int>> Values { get; set; }
    }
}
