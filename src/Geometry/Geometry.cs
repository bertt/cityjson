using CityJSON.Convertors;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace CityJSON.Geometry
{
    [JsonConverter(typeof(GeometryConverter))]
    public abstract class Geometry
    {
        // [JsonConverter(typeof(GeometryTypeConverter))]
        public GeometryType Type { get; set; }
        public string Lod { get; set; }
    }
}
