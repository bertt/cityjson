using CityJSON;
using SharpGLTF.Geometry.VertexTypes;
using SharpGLTF.Geometry;
using SharpGLTF.Materials;
using System.Numerics;
using CityJSON.Geometry;

namespace cj2glb;
public class TexturedGltfCreator
{
    public static byte[] ToGltf(CityJsonDocument cityJsonDocument, string texturesBaseDirectory = "", string id = null)
    {
        var allVertices = cityJsonDocument.Vertices;
        var appearance = cityJsonDocument.Appearance;
        var transform = cityJsonDocument.Transform;

        var scene = new SharpGLTF.Scenes.SceneBuilder();
        var meshBuilder = new MeshBuilder<VertexPosition, VertexTexture1>("mesh");

        var cityObjects = cityJsonDocument.CityObjects;

        if (id != null)
        {
            cityObjects = cityObjects.Where(x => x.Key == id).ToDictionary(x => x.Key, x => x.Value);
        }

        foreach (var cityObject in cityObjects)
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

        if(geometries != null)
        {
            
            foreach (var geom in geometries)
            {
                if(geom != null)
                {
                    if (geom is MultiSurfaceGeometry compositeSolidGeometry)
                    {
                        var multiSurfaceGeometry = (MultiSurfaceGeometry)geom;
                        HandleMultiSurface(multiSurfaceGeometry, meshBuilder, allVertices, appearance, transform, texturesBaseDirectory);
                    }
                    else
                    {
                        // Todo handle other geometry types
                    }
                }
            }
        }
    }

    private static void HandleMultiSurface(MultiSurfaceGeometry multiSurfaceGeometry, MeshBuilder<VertexPosition, VertexTexture1> meshBuilder, List<Vertex> allVertices, Appearance appearance, Transform transform, string texturesBaseDirectory)
    {
        var boundaries = multiSurfaceGeometry.Boundaries;
        if(multiSurfaceGeometry.Texture != null)
        {
            var textures = multiSurfaceGeometry.Texture.First().Value;

            for (var i = 0; i < boundaries.Count(); i++)
            {
                for (var j = 0; j < boundaries[i].Count(); j++)
                {
                    var boundary = boundaries[i][j];
                    var texture = textures[i][j];

                    if (texture[0] != null)
                    {
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

                            // vector2 originates from the bottom left corner
                            t0 = new Vector2(t0.X, 1 - t0.Y);
                            t1 = new Vector2(t1.X, 1 - t1.Y);
                            t2 = new Vector2(t2.X, 1 - t2.Y);

                            var materialText = new MaterialBuilder().WithDoubleSide(true);
                            var image = texturesBaseDirectory + Path.DirectorySeparatorChar + appearance.Textures[imageId].Image;
                            materialText.WithChannelImage(KnownChannel.BaseColor, image);
                            var prim = meshBuilder.UsePrimitive(materialText);
                            prim.AddTriangle((v0, t0), (v1, t1), (v2, t2));
                        }
                    }
                }
            }
        }
    }
}
