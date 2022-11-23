using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;

namespace CityJSON.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var json = File.ReadAllText("fixtures/denhaag.json");
            var cityjson = CityJsonRoot.FromJson(json);
            Assert.IsTrue(cityjson.Version == "1.0");
            Assert.IsTrue(cityjson.CityObjects.Count == 2498);
        }

        [TestMethod]
        public void TestMethod2()
        {
            var json = File.ReadAllText("fixtures/25gn1_04_2020_gebouwen.json");
            var cityjson = CityJsonRoot.FromJson(json);
            Assert.IsTrue(cityjson.Version == "1.0");
            Assert.IsTrue(cityjson.CityObjects.Count == 7313);
        }
    }
}
