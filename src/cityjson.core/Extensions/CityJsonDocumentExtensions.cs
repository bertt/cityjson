using CityJSON.Geometry;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using System.Collections.Generic;

namespace CityJSON.Extensions
{
    public static class CityJsonDocumentExtensions
    {
        public static string ToWkt(this CityJsonDocument cityJson, Transform transform)
        {
            var polygons = new List<Polygon>();
            foreach (var co in cityJson.CityObjects)
            {
                var geom = co.Value.Geometry[0];
                if (geom is MultiSurfaceGeometry)
                {
                    var ms = (MultiSurfaceGeometry)geom;
                    polygons.AddRange(ms.ToPolys(cityJson.Vertices, transform));

                }
                else if (geom is SolidGeometry)
                {
                    var ms = (SolidGeometry)geom;
                    polygons.AddRange(ms.ToPolygons(cityJson.Vertices, transform));
                }
                else
                {
                    // todo add other geometries
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
