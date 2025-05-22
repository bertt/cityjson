using CityJSON;
using CityJSON.Geometry;
using SharpGLTF.Geometry;
using SharpGLTF.Geometry.VertexTypes;
using SharpGLTF.Materials;
using SharpGLTF.Schema2;
using SharpGLTF.Schema2.Tiles3D;
using System.Numerics;

namespace cj2glb;
public class TexturedGltfCreator
{

    private static List<string> GetProperties(List<CityObject> cityObjects)
    {
        var properties = new List<string>();
        foreach (var cityObject in cityObjects)
        {
            if (cityObject.Attributes != null)
            {
                properties.AddRange(cityObject.Attributes.Select(x => x.Key).ToList());
            }
        }
        return properties.Distinct().ToList();
    }
    public static byte[] ToGltf(CityJsonDocument cityJsonDocument, string texturesBaseDirectory = "", string id = null)
    {
        var allVertices = cityJsonDocument.Vertices;
        var appearance = cityJsonDocument.Appearance;
        var transform = cityJsonDocument.Transform;

        Tiles3DExtensions.RegisterExtensions();
        var scene = new SharpGLTF.Scenes.SceneBuilder();
        var meshBuilder = new MeshBuilder<VertexPosition, VertexWithFeatureId1, VertexEmpty>("mesh");

        var cityObjects = cityJsonDocument.CityObjects;

        if (id != null)
        {
            cityObjects = cityObjects.Where(x => x.Key == id).ToDictionary(x => x.Key, x => x.Value);
        }

        var i = 0;
        foreach (var cityObject in cityObjects)
        {
            var co = cityObject.Value;
            ProcessTexturedCityObject(co, meshBuilder, allVertices, appearance, transform, i, texturesBaseDirectory);
            i++;
        }

        scene.AddRigidMesh(meshBuilder, Matrix4x4.Identity);
        var model = scene.ToGltf2();

        
        if(cityObjects.First().Value.Attributes != null)
        {
            var rootMetadata = model.UseStructuralMetadata();
            var schema = rootMetadata.UseEmbeddedSchema("schema_001");

            var schemaClass = schema.UseClassMetadata("triangles");

            var properties = GetProperties(cityObjects.Values.ToList());

            foreach(var property in properties)
            {
                schemaClass.UseProperty(property).WithStringType();
            }
            
            var propertyTable = schemaClass
                .AddPropertyTable(cityObjects.Count);

            var dict = new Dictionary<string, List<string>>();
            foreach (var property in properties)
            {
                dict.Add(property, new List<string>());
            }

            foreach (var cityObject in cityObjects)
            {
                foreach (var property in properties)
                {
                    if (cityObject.Value.Attributes!=null && cityObject.Value.Attributes.ContainsKey(property))
                    {
                        var attribute = Convert.ToString(cityObject.Value.Attributes[property]);
                        dict[property].Add(attribute);
                    }
                    else
                    {
                        dict[property].Add("");
                    }
                }
            }

            foreach (var property in properties)
            {
                var nameProperty = schemaClass.UseProperty(property).WithStringType();
                var values = dict[property].ToArray();
                propertyTable.UseProperty(nameProperty).SetValues(values);
            }

            foreach (var primitive in model.LogicalMeshes[0].Primitives)
            {
                var featureIdAttribute = new FeatureIDBuilder(cityObjects.Count, 0, propertyTable);
                primitive.AddMeshFeatureIds(featureIdAttribute);
            }
        }

        var localTransform = new Matrix4x4(
               1, 0, 0, 0,
               0, 0, -1, 0,
               0, 1, 0, 0,
               0, 0, 0, 1);
        model.LogicalNodes.First().LocalTransform = new SharpGLTF.Transforms.AffineTransform(localTransform);
        var bytes = model.WriteGLB().Array;
        return bytes;
    }

    public static void ProcessTexturedCityObject(CityObject cityObject, MeshBuilder<VertexPosition, VertexWithFeatureId1, VertexEmpty> meshBuilder, List<Vertex> allVertices, Appearance appearance, Transform transform, int featureId, string texturesBaseDirectory = "")
    {
        var geometries = cityObject.Geometry;
        var attributes = cityObject.Attributes;

        if(geometries != null)
        {
            
            foreach (var geom in geometries)
            {
                if(geom != null)
                {
                    if (geom is MultiSurfaceGeometry multiSurfaceGeometry)
                    {
                        HandleMultiSurface(multiSurfaceGeometry, meshBuilder, allVertices, appearance, transform, featureId, texturesBaseDirectory);
                    }
                    else if(geom is SolidGeometry solidGeometry)
                    {
                        HandleSolid(solidGeometry, meshBuilder, allVertices, appearance, transform, featureId, texturesBaseDirectory);
                    }
                    else if(geom is CompositeSurfaceGeometry compositeSurfaceGeometry)
                    {
                        HandleCompositeSurface(compositeSurfaceGeometry, meshBuilder, allVertices, appearance, transform, featureId, texturesBaseDirectory);
                    }
                    else
                    {
                        throw new NotImplementedException($"Geometry type {geom.GetType()} is not implemented.");
                        // Todo handle other geometry types
                    }
                }
            }
        }
    }

    private static void HandleCompositeSurface(CompositeSurfaceGeometry compositeSurfaceGeometry, MeshBuilder<VertexPosition, VertexWithFeatureId1, VertexEmpty> meshBuilder, List<Vertex> allVertices, Appearance appearance, Transform transform, int featureId, string texturesBaseDirectory)
    {
        var boundaries = compositeSurfaceGeometry.Boundaries;

        if (compositeSurfaceGeometry.Texture != null)
        {
            var texture = compositeSurfaceGeometry.Texture.First().Value;

            for (var i = 0; i < boundaries.Count(); i++)
            {
                var boundary = boundaries[i];
                var itexture = texture[i];

                HandleTextures(meshBuilder, allVertices, appearance, transform, texturesBaseDirectory, boundary, itexture, featureId);
            }
        }
    }

    private static void HandleSolid(SolidGeometry solidGeometry, MeshBuilder<VertexPosition, VertexWithFeatureId1, VertexEmpty> meshBuilder, List<Vertex> allVertices, Appearance appearance, Transform transform, int featureId, string texturesBaseDirectory)
    {
        var boundaries = solidGeometry.Boundaries;

        if (solidGeometry.Texture != null)
        {
            var texture = solidGeometry.Texture.First().Value;

            for (var i = 0; i < boundaries.Count(); i++)
            {
                var boundary = boundaries[i];
                var itexture = texture[i];

                HandleTextures(meshBuilder, allVertices, appearance, transform, texturesBaseDirectory, boundary, itexture, featureId);
            }
        }
    }

    private static void HandleMultiSurface(MultiSurfaceGeometry multiSurfaceGeometry, MeshBuilder<VertexPosition, VertexWithFeatureId1, VertexEmpty> meshBuilder, List<Vertex> allVertices, Appearance appearance, Transform transform, int featureId, string texturesBaseDirectory)
    {
        var boundaries = multiSurfaceGeometry.Boundaries;
        if(multiSurfaceGeometry.Texture != null)
        {
            // todo: handle multiple textures
            var firstTexture = multiSurfaceGeometry.Texture.First().Value;

            HandleTextures(meshBuilder, allVertices, appearance, transform, texturesBaseDirectory, boundaries, firstTexture, featureId);
        }
    }

    private static void HandleTextures(MeshBuilder<VertexPosition, VertexWithFeatureId1, VertexEmpty> meshBuilder, List<Vertex> allVertices, Appearance appearance, Transform transform, string texturesBaseDirectory, int[][][] boundaries, int?[][][] firstTexture, int featureId)
    {
        for (var i = 0; i < boundaries.Count(); i++)
        {
            var bnd = boundaries[i];
            var texture = firstTexture[i];

            HandleTextures(meshBuilder, allVertices, appearance, transform, texturesBaseDirectory, bnd, texture, featureId);
        }
    }

    // todo refactor, use MemoryImageCache
    private static PrimitiveBuilder<MaterialBuilder, VertexPosition, VertexWithFeatureId1, VertexEmpty> GetPrimitive(IReadOnlyCollection<PrimitiveBuilder<MaterialBuilder, VertexPosition, VertexWithFeatureId1, VertexEmpty>> primitives, string image)
    {
        foreach (var primitive in primitives)
        {
            var material = primitive.Material;
            var channel = material.GetChannel(KnownChannel.BaseColor);
            if (channel != null)
            {
                var primaryImage = channel.Texture.PrimaryImage;
                var path = primaryImage.Content.SourcePath;
                var image1 = Path.GetFileName(path);
                if (image.Contains(image1))
                {
                    return primitive;
                }
            }
        }
        return null;
    }

    private static void HandleTextures(MeshBuilder<VertexPosition, VertexWithFeatureId1, VertexEmpty> meshBuilder, List<Vertex> allVertices, Appearance appearance, Transform transform, string texturesBaseDirectory, int[][] bnd, int?[][] texture1, int featureId)
    {
        for (var j = 0; j < bnd.Count(); j++)
        {
            var boundary = bnd[j];

            // exception for railway to prevent out of bounds
            if(texture1.Length <= j)
            {
                break;
            }
            var texture = texture1[j];

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

                var image = texturesBaseDirectory + Path.DirectorySeparatorChar + appearance.Textures[imageId].Image;

                var primitives = meshBuilder.Primitives;
                var prim = GetPrimitive(primitives, appearance.Textures[imageId].Image);

                if (prim ==null)
                {
                    var material = new MaterialBuilder().WithDoubleSide(true);
                    material.WithChannelImage(KnownChannel.BaseColor, image);
                    prim = meshBuilder.UsePrimitive(material);
                }

                for (var l = 0; l < indices.Count; l += 3)
                {
                    var index0 = indices[l];
                    var index1 = indices[l + 1];
                    var index2 = indices[l + 2];

                    var v0 = vertices[index0].ToVector3() * transform.ScaleVector3();
                    var v1 = vertices[index1].ToVector3() * transform.ScaleVector3();
                    var v2 = vertices[index2].ToVector3() * transform.ScaleVector3();

                    var t0 = new Vector2(appearance.VerticesTexture[(int)textureIds[index0]]);
                    var t1 = new Vector2(appearance.VerticesTexture[(int)textureIds[index1]]);
                    var t2 = new Vector2(appearance.VerticesTexture[(int)textureIds[index2]]);

                    // vector2 originates from the bottom left corner
                    t0 = new Vector2(t0.X, 1 - t0.Y);
                    t1 = new Vector2(t1.X, 1 - t1.Y);
                    t2 = new Vector2(t2.X, 1 - t2.Y);

                    var vt0 = GetVertexWithFeatureId(v0, featureId, t0);
                    var vt1 = GetVertexWithFeatureId(v1, featureId, t1);
                    var vt2 = GetVertexWithFeatureId(v2, featureId, t2);

                    prim.AddTriangle(vt0, vt1, vt2);
                }
            }
        }
    }


    private static VertexBuilder<VertexPosition, VertexWithFeatureId1, VertexEmpty> GetVertexWithFeatureId(Vector3 position, int featureid, Vector2 texCoord)
    {
        var vp0 = new VertexPosition(position);
        var v = new VertexWithFeatureId1(featureid, texCoord);
        var vb0 = new VertexBuilder<VertexPosition, VertexWithFeatureId1, VertexEmpty>(vp0, v);
        return vb0;
    }
}
