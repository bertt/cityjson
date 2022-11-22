using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
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
            var cityjson = JsonConvert.DeserializeObject<CityJsonRoot>(json);
            Assert.IsTrue(cityjson.Version == "1.0");
            var expected = new List<double>() { 78248.66, 457604.591, 2.463, 79036.024, 458276.439, 37.481 };
            Assert.IsTrue(cityjson.Metadata.GeographicalExtent.Length == expected.ToArray().Length);
        }

        [TestMethod]
        public void TestMethod2()
        {
            var json = File.ReadAllText("fixtures/25gn1_04_2020_gebouwen.json");
            var cityjson = JsonConvert.DeserializeObject<CityJsonRoot>(json);
            Assert.IsTrue(cityjson.Version == "1.0");
            var expected = new List<double>() { 78248.66, 457604.591, 2.463, 79036.024, 458276.439, 37.481 };
            Assert.IsTrue(cityjson.Metadata.GeographicalExtent.Length == expected.ToArray().Length);
        }

    }
}
