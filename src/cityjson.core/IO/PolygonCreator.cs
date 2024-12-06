using System.Collections.Generic;
using CityJSON.Extensions;
using NetTopologySuite.Geometries;

namespace CityJSON.IO
{
    public static class PolygonCreator
    {
        public static Polygon GetPolygon(List<Vertex> vertices, int[] vertexList, Transform transform)
        {
            var coords = new Coordinate[vertexList.Length + 1];
            for (int i = 0; i < vertexList.Length; i++)
            {
                var vertex = vertices[vertexList[i]];
                var coordinate = new CoordinateZ(vertex.X, vertex.Y, vertex.Z);

                coords[i] = coordinate.Transform(transform);
            }
            coords[vertexList.Length] = coords[0];
            var geometryFactory = new GeometryFactory();
            var extRing = geometryFactory.CreateLinearRing(coords);
            var polygon = geometryFactory.CreatePolygon(extRing);
            return polygon;
        }

    }
}
