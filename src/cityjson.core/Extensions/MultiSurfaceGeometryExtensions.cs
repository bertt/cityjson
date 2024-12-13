using System.Collections.Generic;
using CityJSON.Geometry;
using CityJSON.IO;
using NetTopologySuite.Geometries;
using NetTopologySuite.Noding;

namespace CityJSON.Extensions
{
    public static class MultiSurfaceGeometryExtensions
    {
        public static List<Polygon> ToPolys(this MultiSurfaceGeometry multiSurfaceGeometry, List<Vertex> vertices, Transform transform)
        {
            var polygons = new List<Polygon>();

            foreach(var bnd in multiSurfaceGeometry.Boundaries)
            {
                var outer = bnd[0];

                var holes = bnd.Length>1?bnd[1..]:null;

                var poly = PolygonCreator.GetPolygon(vertices, outer, transform, holes);
                polygons.Add(poly);
            }

            return polygons;
        }
    }
}
