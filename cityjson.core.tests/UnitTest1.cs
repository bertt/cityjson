using System.IO;
using System.Linq;
using CityJSON.Extensions;
using Newtonsoft.Json;
using NUnit.Framework;

namespace CityJSON.Tests
{
    public class UnitTest1
    {
        [Test]
        public void ReadBuildingWithInnerRingTest()
        {
            var json = File.ReadAllText("./fixtures/building_with_innerring.city.json");
            var cityjson = JsonConvert.DeserializeObject<CityJsonDocument>(json);

            Assert.That(cityjson.Type, Is.EqualTo("CityJSON"));
            Assert.That(cityjson.Version, Is.EqualTo("1.0"));

            var wkt = cityjson.ToWkt();
            var reader = new NetTopologySuite.IO.WKTReader();
            var geom = reader.Read(wkt);

            Assert.That(geom.GeometryType, Is.EqualTo("MultiPolygon"));
            Assert.That(geom.NumGeometries, Is.EqualTo(102));

        }

        [Test]
        public void ReadCityJsonSeqFileMinimal()
        {
            var jsonSeq = File.ReadAllText("fixtures/tile_00000.city.jsonl");

            var allLines = jsonSeq.Split('\n');

            var firstLine = allLines[0];
            var cityJson = JsonConvert.DeserializeObject<CityJsonDocument>(firstLine);
            Assert.That(cityJson.Type == "CityJSON");
            Assert.That(cityJson.Version == "2.0");

            var transform = cityJson.Transform;

            var secondLine = allLines[1];
            var cityJsonSecond = JsonConvert.DeserializeObject<CityJsonDocument>(secondLine);
            Assert.That(cityJsonSecond.CityObjects.Count == 2);
            var wkt = cityJsonSecond.ToWkt(transform);

            // read with NetTopologySuite
            var reader = new NetTopologySuite.IO.WKTReader();
            var geom = reader.Read(wkt);
            Assert.That(geom.GeometryType == "MultiPolygon");
        }


        [Test]
        public void ReadCityJsonSeqFile()
        {
            var jsonSeq = File.ReadAllText("fixtures/tile_00000.city.jsonl");

            var allLines = jsonSeq.Split('\n');

            var firstLine = allLines[0];
            var cityJson = JsonConvert.DeserializeObject<CityJsonDocument>(firstLine);
            Assert.That(cityJson.Type == "CityJSON");
            Assert.That(cityJson.Version == "2.0");
            
            var transform = cityJson.Transform;
            Assert.That(transform.Scale.Length == 3);
            Assert.That(transform.Scale[0] == 0.01);
            Assert.That(transform.Scale[1] == 0.01);
            Assert.That(transform.Scale[2] == 0.01);
            Assert.That(transform.Translate.Length == 3);
            Assert.That(transform.Translate[0] == 1033078.6);
            Assert.That(transform.Translate[1] == 6280758.8);
            Assert.That(transform.Translate[2] == 0.0);
            Assert.That(cityJson.Vertices.Count == 0);

            var secondLine = allLines[1];
            var cityJsonSecond = JsonConvert.DeserializeObject<CityJsonDocument>(secondLine);
            Assert.That(cityJsonSecond.CityObjects.Count == 2);
            var wkt = cityJsonSecond.ToWkt(transform);

            // read with NetTopologySuite
            var reader = new NetTopologySuite.IO.WKTReader();
            var geom = reader.Read(wkt);
            Assert.That(geom.GeometryType == "MultiPolygon");
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
    }
}
