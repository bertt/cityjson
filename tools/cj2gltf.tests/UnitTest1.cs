using CityJSON;
using Newtonsoft.Json;
using SharpGLTF.Schema2;

namespace cj2gltf.tests;

public class Tests
{
    [TestCase("testfixtures/simplegeom/v2.0/cube.city.json", 12)]
    [TestCase("testfixtures/simplegeom/v2.0/twocube.city.json", 24)]
    [TestCase("testfixtures/simplegeom/v2.0/tetra.city.json", 4)]
    [TestCase("testfixtures/simplegeom/v2.0/torus.city.json", 28)]
    // todo: [TestCase("testfixtures/simplegeom/v2.0/csol.city.json", 12)]
    public void ToGltfNewTests(string filePath, int triangles)
    {
        var json = File.ReadAllText(filePath);
        var cityjson = JsonConvert.DeserializeObject<CityJsonDocument>(json);
        var result = GltfCreator.ToGltf(cityjson);
        Assert.That(result.Count() > 0);
        var stream = new MemoryStream(result);
        var model = ModelRoot.ReadGLB(stream);
        Assert.That(model != null);
        var trianglesActual = Toolkit
          .EvaluateTriangles(model.DefaultScene)
          .ToArray().Count();
        Assert.That(triangles == trianglesActual);
    }
}
