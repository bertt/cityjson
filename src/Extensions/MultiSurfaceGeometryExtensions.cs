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

            foreach(var bnd in multiSurfaceGeometry.Boundaries)
            {
                var poly = PolygonCreator.GetPolygon(vertices, bnd, transform);
                polygons.Add(poly);
            }

            return polygons;
        }
    }
}
