using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CityJSON
{
    public class Semantics
    {
        [JsonPropertyName("values")]
        public List<List<int>> Values { get; set; }

        [JsonPropertyName("surfaces")]
        public List<Surface> Surfaces { get; set; }
    }
}
