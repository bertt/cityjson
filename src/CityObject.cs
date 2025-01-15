using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace CityJSON
{
    public class CityObject
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public CityObjectType Type { get; set; }
        public Dictionary<string, object> Attributes { get; set; }
        public List<Geometry.Geometry> Geometry { get; set; }
    }
}
