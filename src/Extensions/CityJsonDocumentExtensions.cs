using CityJSON.Geometry;
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
            var polygons = new List<Polygon>();
            foreach (var co in cityJson.CityObjects)
            {
                var geoms = lod!=null ? co.Value.Geometry.FindAll(g => g.Lod == lod) : co.Value.Geometry;   

                foreach (var geom in geoms)
                {
                    switch (geom)
                    {
                        case MultiSurfaceGeometry:
                            {
                                var ms = (MultiSurfaceGeometry)geom;
                                polygons.AddRange(ms.ToPolys(cityJson.Vertices, transform));
                                break;
                            }

                        case SolidGeometry:
                            {
                                var ms = (SolidGeometry)geom;
                                polygons.AddRange(ms.ToPolygons(cityJson.Vertices, transform));
                                break;
                            }

                        case MultiSolidGeometry:
                            {
                                var ms = (MultiSolidGeometry)geom;
                                polygons.AddRange(ms.ToPolys(cityJson.Vertices, transform));
                                break;
                            }

                        case CompositeSolidGeometry:
                            {
                                var ms = (CompositeSolidGeometry)geom;
                                polygons.AddRange(ms.ToPolys(cityJson.Vertices, transform));
                                break;
                            }
                    }
                }
            }
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
    }
}
