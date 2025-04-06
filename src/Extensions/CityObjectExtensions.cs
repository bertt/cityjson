using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using System.Collections.Generic;

namespace CityJSON.Extensions;
public static class CityObjectExtensions
{
    public static Feature ToFeature(this CityObject cityObject, List<Vertex> vertices, Transform transform, string lod = null)
    {
        var feature = new Feature();
        var geometries = cityObject.Geometry;

        // Todo support other geometry types
        var polygons = geometries.ToPolygons(vertices, transform, lod);
        var geometryFactory = new GeometryFactory();
        var multiPolygon = geometryFactory.CreateMultiPolygon(polygons.ToArray());
        feature.Geometry = multiPolygon;

        feature.Attributes = new AttributesTable
            {
                { "type", cityObject.Type },
                { "attributes", cityObject.Attributes }
            };

        return feature;
    }
}
