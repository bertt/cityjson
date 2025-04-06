using NetTopologySuite.Features;
using System.Collections.Generic;

namespace CityJSON.Extensions
{
    public static class CityJsonDocumentExtensions
    {
        public static List<Feature> ToFeatures(this CityJsonDocument cityJson, string lod=null)
        {
            return ToFeatures(cityJson, cityJson.Transform,lod);
        }

        public static List<Feature> ToFeatures(this CityJsonDocument cityJson, Transform transform, string lod=null)
        {
            var vertices = cityJson.Vertices;
            var cityObjects = cityJson.CityObjects;

            var result = new List<Feature>(); 

            foreach(var cityObject in cityObjects)
            {
                var co = cityObject.Value;
                var feature = co.ToFeature(vertices, transform, lod);
                result.Add(feature);
            }

            return result;
        }
    }
}
