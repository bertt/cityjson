using System.Collections.Generic;
using CityJSON.Extensions;
using NetTopologySuite.Algorithm;
using NetTopologySuite.Geometries;

namespace CityJSON.IO
{
    public static class PolygonCreator
    {
        public static Polygon GetPolygon(List<Vertex> vertices, int[] vertexList, Transform transform, int[][] holes = null)
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
            Polygon polygon = null;
            if (holes != null)
            {
                foreach (var hole in holes)
                {
                    var holeCoords = new Coordinate[hole.Length + 1];
                    for (int i = 0; i < hole.Length; i++)
                    {
                        var vertex = vertices[hole[i]];
                        var coordinate = new CoordinateZ(vertex.X, vertex.Y, vertex.Z);
                        holeCoords[i] = coordinate.Transform(transform);
                    }
                    holeCoords[hole.Length] = holeCoords[0];
                    var holeRing = geometryFactory.CreateLinearRing(holeCoords);
                    polygon = geometryFactory.CreatePolygon((LinearRing)extRing, new LinearRing[] { holeRing });
                }
            }
            else
            {
                polygon = geometryFactory.CreatePolygon(extRing);
            }
            return polygon;
        }

    }
}
