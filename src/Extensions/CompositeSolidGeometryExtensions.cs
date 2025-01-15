using CityJSON.Geometry;
using NetTopologySuite.Geometries;
using System.Collections.Generic;

namespace CityJSON.Extensions;
public static class CompositeSolidGeometryExtensions
{
    public static List<Polygon> ToPolys(this CompositeSolidGeometry compositeSolidGeometry, List<Vertex> vertices, Transform transform)
    {
        var polygons = new List<Polygon>();

        foreach (var solid in compositeSolidGeometry.Boundaries)
        {
            var solidGeometry = new SolidGeometry();
            solidGeometry.Boundaries = solid;
            var solidPolygons = solidGeometry.ToPolygons(vertices, transform);
            polygons.AddRange(solidPolygons);
        }

        return polygons;
    }

}
