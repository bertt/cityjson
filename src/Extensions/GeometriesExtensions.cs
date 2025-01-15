using NetTopologySuite.Geometries;
using System.Collections.Generic;

namespace CityJSON.Extensions;
public static class GeometriesExtensions
{
    public static List<Polygon> ToPolygons(this List<Geometry.Geometry> geometries, List<Vertex> vertices, Transform transform, string lod = null)
    {
        var polygons = new List<Polygon>();
        var geoms = lod != null ? geometries.FindAll(g => g.Lod == lod) : geometries;

        foreach (var geom in geoms)
        {
            var polys = geom.ToPolygons(vertices, transform);
            polygons.AddRange(polys);
        }
        return polygons;
    }

}
