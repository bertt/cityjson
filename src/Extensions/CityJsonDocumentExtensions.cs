using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using System.Collections.Generic;
using System.Linq;

namespace CityJSON.Extensions
{
    public static class CityJsonDocumentExtensions
    {
        public static Feature ToFeature(this CityJsonDocument cityJson, string lod=null)
        {
            return ToFeature(cityJson, cityJson.Transform,lod);
        }

        public static Feature ToFeature(this CityJsonDocument cityJson, Transform transform, string lod=null)
        {
            var vertices = cityJson.Vertices;
            var cityObjects = cityJson.CityObjects;

            var polygons = GetPolygons(cityObjects, vertices, transform, lod);
            var geometryFactory = new GeometryFactory();

            var multiPolygon = geometryFactory.CreateMultiPolygon(polygons.ToArray());

            var feature = new Feature();

            // select the attributes of the first CityObject
            var attributes = cityJson.CityObjects.First().Value.Attributes;

            if (attributes != null)
            {
                // create new attributes
                var atts = new AttributesTable(attributes);
                feature.Attributes = atts;
            }

            feature.Geometry = multiPolygon;
            return feature;
        }

        private static List<Polygon> GetPolygons(Dictionary<string, CityObject> cityObjects, List<Vertex> vertices, Transform transform, string lod = null)
        {
            var polygons = new List<Polygon>();
            foreach (var co in cityObjects)
            {
                var cityObject = co.Value;
                var polys = cityObject.Geometry.ToPolygons (vertices, transform, lod);
                polygons.AddRange(polys);
            }

            return polygons;
        }


    }
}
