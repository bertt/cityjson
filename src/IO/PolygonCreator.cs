using System.Collections.Generic;
using CityJSON.Extensions;
using NetTopologySuite.Geometries;

namespace CityJSON.IO
{
    public static class PolygonCreator
    {
        public static Polygon GetPolygon(List<Vertex> vertices, int[][] vertexList, Transform transform)
        {
            var hasInterriorRings = vertexList.Length > 1;
            var exteriorRing = vertexList[0];

            if (hasInterriorRings)
            {
                var exterior = GetPolygon(vertices, exteriorRing, transform);
                var interiors = new List<LinearRing>();
                for (int i = 1; i < vertexList.Length; i++)
                {
                    var interiorRing = vertexList[i];
                    var interior = GetPolygon(vertices, interiorRing, transform);
                    interiors.Add((LinearRing)interior.ExteriorRing);
                }
                return new Polygon((LinearRing)exterior.ExteriorRing, interiors.ToArray(), new GeometryFactory());
            }
            else
            {
                return GetPolygon(vertices, exteriorRing, transform);
            }
        }

        private static Polygon GetPolygon(List<Vertex> vertices, int[] ring, Transform transform)
        {
            var coords = new Coordinate[ring.Length + 1];
            for (int i = 0; i < ring.Length; i++)
            {
                var vertex = vertices[ring[i]];
                var coordinate = new CoordinateZ(vertex.X, vertex.Y, vertex.Z);

                coords[i] = coordinate.Transform(transform);
            }
            coords[ring.Length] = coords[0];
            var geometryFactory = new GeometryFactory();
            var extRing = geometryFactory.CreateLinearRing(coords);
            var polygon = geometryFactory.CreatePolygon(extRing);
            return polygon;
        }
    }
}
