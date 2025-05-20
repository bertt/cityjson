using NetTopologySuite.Geometries;
using System.Collections.Generic;

namespace CityJSON
{
    public class CityJsonDocument
    {
        public Metadata Metadata { get; set; }

        public string Type { get; set; } = "CityJSON";

        public string Version { get; set; }

        public Transform Transform { get; set; }

        public Dictionary<string, CityObject> CityObjects { get; set; }

        public List<Vertex> Vertices { get; set; }

        public Appearance Appearance { get; set; }

        public (Envelope envelope, float minZ, float maxZ) GetVerticesEnvelope()
        {
            var minZ = float.MaxValue;
            var maxZ = float.MinValue;

            var envelope = new Envelope();
            foreach (var vertex in Vertices)
            {
                var x = vertex.X * Transform.Scale[0] + Transform.Translate[0];
                var y = vertex.Y * Transform.Scale[1] + Transform.Translate[1];
                var z = vertex.Z * Transform.Scale[2] + Transform.Translate[2];
                var c = new Coordinate(x, y);
                envelope.ExpandToInclude(c);

                if (z < minZ)
                {
                    minZ = (float)z;
                }
                if (z > maxZ)
                {
                    maxZ = (float)z;
                }
            }
            return (envelope, minZ, maxZ);
        }
    }
}
