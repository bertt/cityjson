using System.Collections.Generic;
using CityJSON.Geometry;
using CityJSON.IO;
using NetTopologySuite.Geometries;

namespace CityJSON.Extensions
{
    public static class SolidGeometryExtensions
    {
        public static List<Polygon> ToPolygons(this SolidGeometry solid, List<Vertex> vertices, Transform transform)
        {
            var polygons = new List<Polygon>();
            var bnd0 = solid.Boundaries[0];

            foreach (var outer in bnd0)
            {
                foreach (var outer1 in outer)
                {
                    var poly = PolygonCreator.GetPolygon(vertices, outer1, transform);
                    polygons.Add(poly);
                }
            }
            return polygons;
        }

    }
}
