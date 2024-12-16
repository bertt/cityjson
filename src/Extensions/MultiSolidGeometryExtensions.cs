using CityJSON.Geometry;
using NetTopologySuite.Geometries;
using System.Collections.Generic;

namespace CityJSON.Extensions;
public static class MultiSolidGeometryExtensions
{
    public static List<Polygon> ToPolys(this MultiSolidGeometry multiSolidGeometry, List<Vertex> vertices, Transform transform)
    {
        var polygons = new List<Polygon>();

        foreach(var solid in multiSolidGeometry.Boundaries)
        {
            // create a solid geometry
            var solidGeometry = new SolidGeometry();
            solidGeometry.Boundaries = solid;
            var solidPolygons = solidGeometry.ToPolygons(vertices, transform);
            polygons.AddRange(solidPolygons);
        }

        return polygons;
    }
}
