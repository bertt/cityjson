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
            var polygon = PolygonCreator.GetPolygon(vertices, poly, transform);
            polygons.Add(polygon);
        }

        return polygons;
    }

}
