using System.IO;
using NUnit.Framework;

namespace CityJSON.Tests
{
    public class UnitTest1
    {
        [Test]
        public void ReadMinimal20CityJson()
        {
            var json = File.ReadAllText("fixtures/minimal.city.json");
            var cityjson = CityJsonRoot.FromJson(json);

            Assert.That(cityjson.Type == CityJsonRootType.CityJson);
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
            Assert.That(cityjson.Vertices.Length== 0);
        }


        [Test]
        public void TestMethod1()
        {
            var json = File.ReadAllText("fixtures/denhaag.json");
            var cityjson = CityJsonRoot.FromJson(json);
            Assert.That(cityjson.Version == "1.0");
            Assert.That(cityjson.CityObjects.Count == 2498);
        }

        [Test]
        public void TestMethod2()
        {
            var json = File.ReadAllText("fixtures/25gn1_04_2020_gebouwen.json");
            var cityjson = CityJsonRoot.FromJson(json);
            Assert.That(cityjson.Version == "1.0");
            Assert.That(cityjson.CityObjects.Count == 7313);
        }
    }
}
