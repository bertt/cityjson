using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CityJSON
{
    public class Surface
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("Direction")]
        public double? Direction { get; set; } 

        [JsonPropertyName("Slope")]
        public double? Slope { get; set; }     }
}
