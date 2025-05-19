using CityJSON;
using cj2glb;
using Newtonsoft.Json;
using SharpGLTF.Schema2;

namespace cj2gltf.tests;

public class Tests
{
    [TestCase("testfixtures/railway/LoD3_Railway.city.json", 26797)]
    public void RailwayTextureToGltfTest(string filePath, int triangles)
    {
        var texturesBaseDirectory = "./testfixtures/railway";
        var json = File.ReadAllText(filePath);
        var cityjson = JsonConvert.DeserializeObject<CityJsonDocument>(json);
        var result = TexturedGltfCreator.ToGltf(cityjson!, texturesBaseDirectory);
        Assert.That(result.Count() > 0);
        var stream = new MemoryStream(result);
        var model = ModelRoot.ReadGLB(stream);
        Assert.That(model != null);
        var trianglesActual = Toolkit
          .EvaluateTriangles(model!.DefaultScene)
          .ToArray().Count();
        Assert.That(triangles == trianglesActual);
    }

    [TestCase("testfixtures/fzkhaus/01-FZK-Haus-LoD2-Tex-no-Storey-CG30-V01_formatted.json", 20)]
    public void FzkHausTextureToGltfTest(string filePath, int triangles)
    {
        var texturesBaseDirectory = "./testfixtures/fzkhaus";
        var json = File.ReadAllText(filePath);
        var cityjson = JsonConvert.DeserializeObject<CityJsonDocument>(json);
        var result = TexturedGltfCreator.ToGltf(cityjson!, texturesBaseDirectory);
        Assert.That(result.Count() > 0);
        var stream = new MemoryStream(result);
        var model = ModelRoot.ReadGLB(stream);
        Assert.That(model != null);
        var trianglesActual = Toolkit
          .EvaluateTriangles(model!.DefaultScene)
          .ToArray().Count();
        Assert.That(triangles == trianglesActual);
    }

    [TestCase("testfixtures/3-20-DELFSHAVEN.city.json", 6)]
    public void TextureToGltfTest(string filePath, int triangles)
    {
        var texturesBaseDirectory = "./testfixtures/";
        var json = File.ReadAllText(filePath);
        var cityjson = JsonConvert.DeserializeObject<CityJsonDocument>(json);
        var id = "{CCADD505-2C1A-409F-8D4B-793A374DB47A}";
        var result = TexturedGltfCreator.ToGltf(cityjson!, texturesBaseDirectory, id);
        Assert.That(result.Count() > 0);
        var stream = new MemoryStream(result);
        var model = ModelRoot.ReadGLB(stream);
        Assert.That(model != null);
        var trianglesActual = Toolkit
          .EvaluateTriangles(model!.DefaultScene)
          .ToArray().Count();
        Assert.That(triangles == trianglesActual);
    }

    [TestCase("testfixtures/simplegeom/v2.0/cube.city.json", 12)]
    [TestCase("testfixtures/simplegeom/v2.0/twocube.city.json", 24)]
    [TestCase("testfixtures/simplegeom/v2.0/tetra.city.json", 4)]
    [TestCase("testfixtures/simplegeom/v2.0/torus.city.json", 28)]
    [TestCase("testfixtures/simplegeom/v2.0/csol.city.json", 24)]
    [TestCase("testfixtures/simplegeom/v2.0/msol.city.json", 24)]
    public void ToGltfNewTests(string filePath, int triangles)
    {
        var json = File.ReadAllText(filePath);
        var cityjson = JsonConvert.DeserializeObject<CityJsonDocument>(json);
        var result = GltfCreator.ToGltf(cityjson!);
        Assert.That(result.Count() > 0);
        var stream = new MemoryStream(result);
        var model = ModelRoot.ReadGLB(stream);
        Assert.That(model != null);
        var trianglesActual = Toolkit
          .EvaluateTriangles(model!.DefaultScene)
          .ToArray().Count();
        Assert.That(triangles == trianglesActual);
    }

    [TestCase("testfixtures/3-20-DELFSHAVEN.city.json")]
    public void DelfshavenToGltfNewTests(string filePath)
    {
        var json = File.ReadAllText(filePath);
        var cityjson = JsonConvert.DeserializeObject<CityJsonDocument>(json);
        var result = GltfCreator.ToGltf(cityjson!);
        Assert.That(result.Count() > 0);
        var stream = new MemoryStream(result);
        var model = ModelRoot.ReadGLB(stream);
        Assert.That(model != null);
        var trianglesActual = Toolkit
          .EvaluateTriangles(model!.DefaultScene)
          .ToArray().Count();
    }
}
