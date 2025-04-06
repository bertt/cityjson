using System.IO;
using System.Linq;
using CityJSON.Extensions;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Newtonsoft.Json;
using NUnit.Framework;

namespace CityJSON.Tests
{


    public class UnitTest1
    {
        [Test]
        public void MultiPolyTest()
        {
            var geometryFactory = new GeometryFactory();

            // Buitenste polygoon (exterior)
            var exterior = new LinearRing(new[]
            {
            new Coordinate(0, 0),
            new Coordinate(10, 0),
            new Coordinate(10, 10),
            new Coordinate(0, 10),
            new Coordinate(0, 0)  // Sluit de ring
        });

            // Binnenste gat (interior ring)
            var hole = new LinearRing(new[]
            {
            new Coordinate(3, 3),
            new Coordinate(7, 3),
            new Coordinate(7, 7),
            new Coordinate(3, 7),
            new Coordinate(3, 3)  // Sluit de ring
        });

            // Maak de polygon met het gat
            var polygonWithHole = new Polygon(exterior, new[] { hole }, geometryFactory);

            // Maak een Multipolygon met één polygon met een gat
            var multiPolygon1 = new MultiPolygon(new[] { polygonWithHole }, geometryFactory);

        }

        [Test]
        public void TestTorus()
        {
            var json = File.ReadAllText("./fixtures/simplegeom/v2.0/cube.city.json");
            var cityjson = JsonConvert.DeserializeObject<CityJsonDocument>(json);
            var features = cityjson.ToFeatures();
            Assert.That(features.Count == 1);
            var feature = features.First();
            var multiPolygon = (MultiPolygon)feature.Geometry;
            var polys = multiPolygon.Geometries;
            // multiPolygon.Geometries.

        }



        [Test]
        public void ReadDelftshavenTest()
        {
            var json = File.ReadAllText("./fixtures/3-20-DELFSHAVEN.city.json");
            var cityjson = JsonConvert.DeserializeObject<CityJsonDocument>(json);
            var firstCityObject = cityjson.CityObjects.First().Value;
            Assert.That(firstCityObject.Type == CityObjectType.Building);
            var firstGeometry = firstCityObject.Geometry.First();
            Assert.That(firstGeometry.Type == Geometry.GeometryType.MultiSurface);
            var texture = firstGeometry.Texture;
            Assert.That(texture != null);
            Assert.That(texture.Count == 1);
            Assert.That(texture.First().Key == "rgbTexture");
            var firstTexture = texture.First().Value;
            Assert.That(firstTexture[0][0][0] == 50);
            Assert.That(firstTexture[0][0][1] == 15254);
            Assert.That(firstTexture[0][0][2] == 15255);
            Assert.That(firstTexture[0][0][3] == 15256);
            Assert.That(firstTexture[0][0][4] == 15257);
        }

        [Test]
        public void ReadAppearanceTest()
        {
            var json = File.ReadAllText("./fixtures/minimal_textures.city.json");
            var cityjson = JsonConvert.DeserializeObject<CityJsonDocument>(json);
            Assert.That(cityjson.Appearance.Textures.Count == 2);
            var firstTexture = cityjson.Appearance.Textures.First();
            Assert.That(firstTexture.Image == "http://www.someurl.org/filename.jpg");
            Assert.That(firstTexture.ImageType == TextureImageType.PNG);
            var secondTexture = cityjson.Appearance.Textures.Last();
            Assert.That(secondTexture.Image == "appearances/myroof.jpg");
            Assert.That(secondTexture.ImageType == TextureImageType.JPG);
            Assert.That(secondTexture.TextureType == TextureType.unknown);
            Assert.That(secondTexture.WrapMode == TextureWrapMode.wrap);
            Assert.That(secondTexture.BorderColor.Count() == 4);
            var verticesTexture = cityjson.Appearance.VerticesTexture;
            Assert.That(verticesTexture.Count() == 4);
            Assert.That(verticesTexture[0].Count() == 2);
            Assert.That(verticesTexture[0][0] == 0.0f);
            Assert.That(verticesTexture[0][1] == 0.5f);
        }

        [Test]
        public void ReadBuildingWithInnerRingTest()
        {
            var json = File.ReadAllText("./fixtures/building_with_innerring.city.json");
            var cityjson = JsonConvert.DeserializeObject<CityJsonDocument>(json);

            Assert.That(cityjson.Type, Is.EqualTo("CityJSON"));
            Assert.That(cityjson.Version, Is.EqualTo("1.0"));

            var features = cityjson.ToFeatures();
            Assert.That(features.Count, Is.EqualTo(1));
            var feature = features.First();
            var geom = feature.Geometry;

            Assert.That(geom.GeometryType, Is.EqualTo("MultiPolygon"));
            Assert.That(geom.NumGeometries, Is.EqualTo(102));

        }

        [Test]
        public void ReadMinimal20CityJson()
        {
            var json = File.ReadAllText("fixtures/minimal.city.json");
            var cityjson = JsonConvert.DeserializeObject<CityJsonDocument>(json);

            Assert.That(cityjson.Type == "CityJSON");
            Assert.That(cityjson.Version == "2.0");
            Assert.That(cityjson.CityObjects.Count == 0);

            Assert.That(cityjson.Transform.Scale.Length == 3);
            Assert.That(cityjson.Transform.Scale[0] == 1);
            Assert.That(cityjson.Transform.Scale[1] == 1);
            Assert.That(cityjson.Transform.Scale[2] == 1);

            Assert.That(cityjson.Transform.Translate.Length == 3);
            Assert.That(cityjson.Transform.Translate[0] == 0);
            Assert.That(cityjson.Transform.Translate[1] == 0);
            Assert.That(cityjson.Transform.Translate[2] == 0);

            Assert.That(cityjson.CityObjects.Count == 0);
            Assert.That(cityjson.Vertices.Count == 0);
        }


        [Test]
        public void TestReadDenHaag()
        {
            var json = File.ReadAllText("fixtures/denhaag.json");
            var cityjson = JsonConvert.DeserializeObject<CityJsonDocument>(json);
            Assert.That(cityjson.Version == "1.0");
            Assert.That(cityjson.CityObjects.Count == 2498);
            var firstCityObject = cityjson.CityObjects.First().Value;
            var attributes = firstCityObject.Attributes;
            Assert.That(attributes.Count == 5);
            var firstAttribute = attributes.First().Value;
            Assert.That(firstAttribute.Equals("1000"));

            var firstGeometry = firstCityObject.Geometry.First();
            Assert.That(firstGeometry.Type == Geometry.GeometryType.Solid);

            Assert.That(firstGeometry.Lod == "2");
        }

        [Test]
        public void TestReadGebouwen()
        {
            var json = File.ReadAllText("fixtures/25gn1_04_2020_gebouwen.json");
            var cityjson = JsonConvert.DeserializeObject<CityJsonDocument>(json);
            Assert.That(cityjson.Version == "1.0");
            Assert.That(cityjson.CityObjects.Count == 7313);
        }

        [Test]
        public void TestMesh()
        {
            var json = File.ReadAllText("fixtures/mesh.json");
            var cityjson = JsonConvert.DeserializeObject<CityJsonDocument>(json);
            Assert.That(cityjson.Version == "2.0");
            Assert.That(cityjson.CityObjects.Count == 1);
            var firstCityObject = cityjson.CityObjects.First();
            var cityObject = firstCityObject.Value;
            Assert.That(cityObject.Type == CityObjectType.TINRelief);
            var transform = cityjson.Transform;
            var vertices = cityjson.Vertices;
            Assert.That(cityObject.Geometry.Count == 1);

            var polys = cityObject.Geometry.ToPolygons(vertices, transform);
            Assert.That(polys.Count == 82701);
            var feature = cityObject.ToFeature(vertices, transform);

            var wktWriter = new WKTWriter();
            wktWriter.OutputOrdinates = Ordinates.XYZ;

            var wkt = wktWriter.Write(feature.Geometry);
            Assert.That(((MultiPolygon)feature.Geometry).Count == 82701);

        }
    }
}
