using System.IO;
using System.Linq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace CityJSON.Tests
{
    public class UnitTest1
    {
        [Test]
        public void ReadMinimal20CityJson()
        {
            var json = File.ReadAllText("fixtures/minimal.city.json");

            // convert to object using newtonsoft
            var cityjson = JsonConvert.DeserializeObject<CityJson>(json);

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
        public void TestMethod1()
        {
            var json = File.ReadAllText("fixtures/denhaag.json");
            var cityjson = JsonConvert.DeserializeObject<CityJson>(json);
            Assert.That(cityjson.Version == "1.0");
            Assert.That(cityjson.CityObjects.Count == 2498);
            var firstCityObject = cityjson.CityObjects.First().Value;
            var attributes = firstCityObject.Attributes;
            Assert.That(attributes.Count == 5);
            var firstAttribute = attributes.First().Value;
            Assert.That(firstAttribute.Equals("1000"));

            var firstGeometry = firstCityObject.Geometry.First();
            Assert.That(firstGeometry.Type == "Solid");

            Assert.That(firstGeometry.Lod == 2);



        }

        // [Test]
        public void TestMethod2()
        {
            var json = File.ReadAllText("fixtures/25gn1_04_2020_gebouwen.json");
            var cityjson = JsonConvert.DeserializeObject<CityJson>(json);
            Assert.That(cityjson.Version == "1.0");
            Assert.That(cityjson.CityObjects.Count == 7313);
        }
    }
}
