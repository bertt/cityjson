using CityJSON.Geometry;
using CityJSON.IO;
using NetTopologySuite.Geometries;
using System.Collections.Generic;

namespace CityJSON.Extensions;
public static class CompositeSurfaceGeometryExtensions
{
    public static List<Polygon> ToPolygons(this CompositeSurfaceGeometry compositeSurfaceGeometry, List<Vertex> vertices, Transform transform)
    {
        var polygons = new List<Polygon>();

        foreach (var poly in compositeSurfaceGeometry.Boundaries)
        {
            var outer = poly[0];
            var holes = poly.Length > 1 ? poly[1..] : null;
            var polygon = PolygonCreator.GetPolygon(vertices, outer, transform, holes);
            polygons.Add(polygon);
        }

        return polygons;
    }

}
