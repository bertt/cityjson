using System.Collections.Generic;

namespace CityJSON
{
    public class CityObject
    {
        public CityObjectType Type { get; set; }
        public Dictionary<string, object> Attributes { get; set; }
        public List<Geometry.Geometry> Geometry { get; set; }
    }
}
