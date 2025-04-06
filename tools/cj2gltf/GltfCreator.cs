using CityJSON;
using SharpGLTF.Geometry;
using SharpGLTF.Materials;
using System.Numerics;
using Triangulate;
using Wkx;
using CityJSON.Extensions;

namespace cj2gltf;

using VERTEX = SharpGLTF.Geometry.VertexTypes.VertexPosition;

public static class GltfCreator
{
    public static byte[] ToGltf(CityJsonDocument cityjsonDocument)
    {
        var transform = cityjsonDocument!.Transform;
        var feature = cityjsonDocument.ToFeature(transform);
        var wkt = feature.Geometry.AsText();
        var g = (MultiPolygon)Geometry.Deserialize<WktSerializer>(wkt);
        var wkb = g.AsBinary();
        var wkbTriangulated = Triangulator.Triangulate(wkb);
        var triangulatedGeometry = (MultiPolygon)Geometry.Deserialize<WkbSerializer>(wkbTriangulated);
        var bytes = CreateGltf(triangulatedGeometry.Geometries);
        return bytes;
    }
    public static byte[] CreateGltf(List<Polygon> geometries)
    {
        // convert to glTF to be able to inspect the result...
        var material1 = new MaterialBuilder()
           .WithDoubleSide(true)
           .WithMetallicRoughnessShader()
           .WithChannelParam(KnownChannel.BaseColor, KnownProperty.RGBA, new Vector4(0.7f, 0, 0f, 1.0f))
           .WithEmissive(new Vector3(0.2f, 0.3f, 0.1f));

        var mesh = new MeshBuilder<VERTEX>("mesh");
        var prim = mesh.UsePrimitive(material1);
        foreach (var t in geometries)
        {
            prim.AddTriangle(
            new VERTEX((float)t.ExteriorRing.Points[0].X, (float)t.ExteriorRing.Points[0].Y, (float)t.ExteriorRing.Points[0].Z),
            new VERTEX((float)t.ExteriorRing.Points[1].X, (float)t.ExteriorRing.Points[1].Y, (float)t.ExteriorRing.Points[1].Z),
                new VERTEX((float)t.ExteriorRing.Points[2].X, (float)t.ExteriorRing.Points[2].Y, (float)t.ExteriorRing.Points[2].Z));
        }

        var scene = new SharpGLTF.Scenes.SceneBuilder();
        scene.AddRigidMesh(mesh, Matrix4x4.Identity);
        var model = scene.ToGltf2();

        var localTransform = new Matrix4x4(
               1, 0, 0, 0,
               0, 0, -1, 0,
               0, 1, 0, 0,
               0, 0, 0, 1);
        model.LogicalNodes.First().LocalTransform = new SharpGLTF.Transforms.AffineTransform(localTransform);

        var bytes = model.WriteGLB();
        return bytes.Array;
    }
}
