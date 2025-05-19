using NetTopologySuite.Geometries;
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

        public Appearance Appearance { get; set; }

        public Envelope GetVerticesEnvelope()
        {
            var envelope = new Envelope();
            foreach (var vertex in Vertices)
            {
                var x = vertex.X * Transform.Scale[0] + Transform.Translate[0];
                var y = vertex.Y * Transform.Scale[1] + Transform.Translate[1];
                var z = vertex.Z * Transform.Scale[2] + Transform.Translate[2];
                var c = new CoordinateZ(x, y, z);
                envelope.ExpandToInclude(c);
            }
            return envelope;
        }

    }
}
