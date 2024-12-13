using CityJSON.Geometry;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using System.Collections.Generic;

namespace CityJSON.Extensions
{
    public static class CityJsonDocumentExtensions
    {
        public static string ToWkt(this CityJsonDocument cityJson, string lod=null)
        {
            return ToWkt(cityJson, cityJson.Transform,lod);
        }

        public static string ToWkt(this CityJsonDocument cityJson, Transform transform, string lod=null)
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
            var wktWriter = new WKTWriter();
            wktWriter.OutputOrdinates = Ordinates.XYZ;
            var wkt = wktWriter.Write(multiPolygon);
            return wkt;
        }
    }
}
