using CityJSON.Extensions;
using Newtonsoft.Json;
using NUnit.Framework;
using System.IO;

namespace CityJSON.Tests;
public class SimpleGeometriesTests
{
    [TestCase("fixtures/simplegeom/v2.0/twocube.city.json", "CityJSON", "2.0", "MultiPolygon", 12)]
    [TestCase("fixtures/simplegeom/v2.0/csol.city.json", "CityJSON", "2.0", "MultiPolygon", 12)]
    [TestCase("fixtures/simplegeom/v2.0/msol.city.json", "CityJSON", "2.0", "MultiPolygon", 12)]
    [TestCase("fixtures/simplegeom/v2.0/torus.city.json", "CityJSON", "2.0", "MultiPolygon", 11)]
    [TestCase("fixtures/simplegeom/v2.0/tetra.city.json", "CityJSON", "2.0", "MultiPolygon", 4)]
    [TestCase("fixtures/simplegeom/v2.0/cube.city.json", "CityJSON", "2.0", "MultiPolygon", 6)]
    public void ValidateCityJsonFile(string filePath, string expectedType, string expectedVersion, string expectedGeometryType, int expectedNumGeometries)
    {
        var json = File.ReadAllText(filePath);
        var cityjson = JsonConvert.DeserializeObject<CityJsonDocument>(json);

        Assert.That(cityjson.Type, Is.EqualTo(expectedType));
        Assert.That(cityjson.Version, Is.EqualTo(expectedVersion));

        var wkt = cityjson.ToWkt();
        var reader = new NetTopologySuite.IO.WKTReader();
        var geom = reader.Read(wkt);

        Assert.That(geom.GeometryType, Is.EqualTo(expectedGeometryType));
        Assert.That(geom.NumGeometries, Is.EqualTo(expectedNumGeometries));
    }
}
