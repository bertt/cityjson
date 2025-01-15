using NetTopologySuite.Features;
using System.Collections.Generic;

namespace CityJSON.Extensions;
public static class CityObjectExtensions
{
    public static List<Feature> ToFeatures(this CityObject cityObject, List<Vertex> vertices, Transform transform, string lod=null)
    {
        var features = new List<Feature>();
        var geometries = cityObject.Geometry;
        
        var polygons = geometries.ToPolygons(vertices, transform, lod);

        foreach ( var polygon in polygons ) {
            var feature = new Feature();
            feature.Geometry = polygon;
            feature.Attributes = new AttributesTable();
            feature.Attributes.Add("type", cityObject.Type);
            feature.Attributes.Add("attributes", cityObject.Attributes);
            features.Add(feature);
        }

        return features;
    }
}
