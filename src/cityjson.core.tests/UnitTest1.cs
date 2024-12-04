using System.IO;
using NUnit.Framework;

namespace CityJSON.Tests
{
    public class UnitTest1
    {
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
