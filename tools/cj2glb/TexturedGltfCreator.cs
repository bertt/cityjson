using CityJSON;
using SharpGLTF.Geometry.VertexTypes;
using SharpGLTF.Geometry;
using SharpGLTF.Materials;
using System.Numerics;
using CityJSON.Geometry;

namespace cj2glb;
public class TexturedGltfCreator
{
    public static byte[] ToGltf(CityJsonDocument cityJsonDocument, string texturesBaseDirectory = "")
    {
        var allVertices = cityJsonDocument.Vertices;
        var appearance = cityJsonDocument.Appearance;
        var transform = cityJsonDocument.Transform;

        var scene = new SharpGLTF.Scenes.SceneBuilder();
        var meshBuilder = new MeshBuilder<VertexPosition, VertexTexture1>("mesh");

        foreach (var cityObject in cityJsonDocument.CityObjects)
        {
            var co = cityObject.Value;
            ProcessTexturedCityObject(co, meshBuilder, allVertices, appearance, transform, texturesBaseDirectory);
        }

        scene.AddRigidMesh(meshBuilder, Matrix4x4.Identity);
        var model = scene.ToGltf2();

        var localTransform = new Matrix4x4(
               1, 0, 0, 0,
               0, 0, -1, 0,
               0, 1, 0, 0,
               0, 0, 0, 1);
        model.LogicalNodes.First().LocalTransform = new SharpGLTF.Transforms.AffineTransform(localTransform);
        var bytes = model.WriteGLB().Array;
        return bytes;
    }

    public static void ProcessTexturedCityObject(CityObject cityObject, MeshBuilder<VertexPosition, VertexTexture1> meshBuilder, List<Vertex> allVertices, Appearance appearance, Transform transform, string texturesBaseDirectory = "")
    {
        var geometries = cityObject.Geometry;

        foreach (var geom in geometries)
        {
            // Assume MultiSurface, todo add support for other types
            var multiSurfaceGeometry = (MultiSurfaceGeometry)geom;
            var boundaries = multiSurfaceGeometry.Boundaries;
            var textures = multiSurfaceGeometry.Texture.First().Value;

            for (var i = 0; i < boundaries.Count(); i++)
            {
                for (var j = 0; j < boundaries[i].Count(); j++)
                {
                    var boundary = boundaries[i][j];
                    var texture = textures[i][j];

                    var imageId = (int)texture[0];
                    var textureIds = texture.Skip(1).ToArray();

                    var vertices = new List<Vertex>();
                    foreach (var vertex in boundary)
                    {
                        vertices.Add(allVertices[vertex]);
                    }
                    // todo: handle interior rings
                    var interiorRings = new List<int>();
                    var indices = Tesselator.Tesselate(vertices, interiorRings);

                    for (var l = 0; l < indices.Count; l += 3)
                    {
                        var index0 = indices[l];
                        var index1 = indices[l + 1];
                        var index2 = indices[l + 2];

                        var v0 = vertices[index0].ToVector3() * transform.ScaleVector3() + transform.TranslateVector3();
                        var v1 = vertices[index1].ToVector3() * transform.ScaleVector3() + transform.TranslateVector3();
                        var v2 = vertices[index2].ToVector3() * transform.ScaleVector3() + transform.TranslateVector3();

                        var t0 = new Vector2(appearance.VerticesTexture[(int)textureIds[index0]]);
                        var t1 = new Vector2(appearance.VerticesTexture[(int)textureIds[index1]]);
                        var t2 = new Vector2(appearance.VerticesTexture[(int)textureIds[index2]]);

                        var materialText = new MaterialBuilder().WithDoubleSide(true);
                        var image = texturesBaseDirectory + appearance.Textures[imageId].Image;
                        materialText.WithChannelImage(KnownChannel.BaseColor, image);
                        var prim = meshBuilder.UsePrimitive(materialText);
                        prim.AddTriangle((v0, t0), (v1, t1), (v2, t2));
                    }
                }
            }
        }
    }
}
