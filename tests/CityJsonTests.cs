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

            var feature = cityjson.ToFeature();
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
            var features = cityObject.ToFeatures(vertices, transform);

            var wktWriter = new WKTWriter();
            wktWriter.OutputOrdinates = Ordinates.XYZ;

            var feature = features.First();
            var wkt = wktWriter.Write(feature.Geometry);

            Assert.That(wkt == "POLYGON Z((76258.92 369305.04 24.5, 76262.22 369315.44 24.5, 76254.42 369311.33999999997 24.5, 76258.92 369305.04 24.5))");
        }
    }
}
