using System.Collections.Generic;

namespace CityJSON
{
    public class CityJsonDocument
    {
        public string Type { get; set; } = "CityJSON";

        public string Version { get; set; }

        public Transform Transform { get; set; }

        public Dictionary<string, CityObject> CityObjects { get; set; }

        public List<Vertex> Vertices { get; set; }
    }
}
