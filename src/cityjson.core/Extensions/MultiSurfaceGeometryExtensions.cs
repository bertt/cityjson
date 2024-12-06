using System.Collections.Generic;
using CityJSON.Geometry;
using CityJSON.IO;
using NetTopologySuite.Geometries;

namespace CityJSON.Extensions
{
    public static class MultiSurfaceGeometryExtensions
    {
        public static List<Polygon> ToPolys(this MultiSurfaceGeometry multiSurfaceGeometry, List<Vertex> vertices, Transform transform)
        {
            var polygons = new List<Polygon>();
            var bnd0 = multiSurfaceGeometry.Boundaries[0];
            var outer0 = bnd0[0];

            var poly = PolygonCreator.GetPolygon(vertices, outer0, transform);

            polygons.Add(poly);
            return polygons;
        }
    }
}
