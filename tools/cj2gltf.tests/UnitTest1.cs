﻿using CityJSON;
using Newtonsoft.Json;
using SharpGLTF.Schema2;

namespace cj2gltf.tests;

public class Tests
{
    [TestCase("testfixtures/simplegeom/v2.0/twocube.city.json", "CityJSON", "2.0", "MultiPolygon", 12)]
    [TestCase("testfixtures/simplegeom/v2.0/csol.city.json", "CityJSON", "2.0", "MultiPolygon", 12)]
    [TestCase("testfixtures/simplegeom/v2.0/msol.city.json", "CityJSON", "2.0", "MultiPolygon", 12)]
    [TestCase("testfixtures/simplegeom/v2.0/torus.city.json", "CityJSON", "2.0", "MultiPolygon", 11)]
    [TestCase("testfixtures/simplegeom/v2.0/tetra.city.json", "CityJSON", "2.0", "MultiPolygon", 4)]
    [TestCase("testfixtures/simplegeom/v2.0/cube.city.json", "CityJSON", "2.0", "MultiPolygon", 6)]
    public void Convert2GltfTests(string filePath, string expectedType, string expectedVersion, string expectedGeometryType, int expectedNumGeometries)
    {
        var json = File.ReadAllText(filePath);
        var cityjson = JsonConvert.DeserializeObject<CityJsonDocument>(json);

        var result = GltfCreator.ToGltf(cityjson);
        Assert.That(result.Count() > 0);
        var stream = new MemoryStream(result);
        var model = ModelRoot.ReadGLB(stream);
        Assert.That(model != null); 
        // model.SaveGLB(@"torus.glb");
    }
}
