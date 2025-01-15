using CityJSON.Geometry;
using NetTopologySuite.Geometries;
using System.Collections.Generic;

namespace CityJSON.Extensions;
public static class GeometryExtensions
{
    public static List<Polygon> ToPolygons(this Geometry.Geometry geom, List<Vertex> vertices, Transform transform)
    {
        var polygons = new List<Polygon>();
        switch (geom)
        {
            case MultiSurfaceGeometry:
                {
                    var ms = (MultiSurfaceGeometry)geom;
                    polygons.AddRange(ms.ToPolys(vertices, transform));
                    break;
                }

            case SolidGeometry:
                {
                    var ms = (SolidGeometry)geom;
                    polygons.AddRange(ms.ToPolygons(vertices, transform));
                    break;
                }

            case MultiSolidGeometry:
                {
                    var ms = (MultiSolidGeometry)geom;
                    polygons.AddRange(ms.ToPolys(vertices, transform));
                    break;
                }

            case CompositeSolidGeometry:
                {
                    var ms = (CompositeSolidGeometry)geom;
                    polygons.AddRange(ms.ToPolys(vertices, transform));
                    break;
                }
            case CompositeSurfaceGeometry:
                {
                    var ms = (CompositeSurfaceGeometry)geom;
                    polygons.AddRange(ms.ToPolygons(vertices, transform));
                    break;
                }
        }

        return polygons;
    }

}
