using CityJSON;
using SharpGLTF.Geometry.VertexTypes;
using SharpGLTF.Geometry;
using SharpGLTF.Materials;
using System.Numerics;
using CityJSON.Geometry;
using EarcutNet;

namespace cj2gltf;
public static class GltfCreator
{
    public static byte[] ToGltf(CityJsonDocument cityJsonDocument)
    {
        var scene = new SharpGLTF.Scenes.SceneBuilder();
        var meshBuilder = new MeshBuilder<VertexPosition>("mesh");
        var material1 = new MaterialBuilder()
           .WithDoubleSide(true)
           .WithMetallicRoughnessShader()
           .WithChannelParam(KnownChannel.BaseColor, KnownProperty.RGBA, new Vector4(0.7f, 0, 0f, 1.0f))
           .WithEmissive(new Vector3(0.2f, 0.3f, 0.1f));

        var prim = meshBuilder.UsePrimitive(material1);

        var allVertices = cityJsonDocument.Vertices;
        var appearance = cityJsonDocument.Appearance;
        var transform = cityJsonDocument.Transform;

        var translate = transform.TranslateVector3();
        var scale = transform.ScaleVector3();

        foreach (var co in cityJsonDocument.CityObjects)
        {
            var cityObject = co.Value;
            var geometries = cityObject.Geometry;

            for (var i = 0; i < geometries.Count; i++)
            {
                var geometry = geometries[i];
                // Composite Geometry
                var solidGeometry = (SolidGeometry)geometry;
                var boundaries = solidGeometry.Boundaries[0];

                for (var j = 0; j < boundaries.Length; j++)
                {
                    var boundary = boundaries[j];

                    var hasInterriorRings = boundary.Length > 1;
                    var exteriorRing = boundary[0];

                    var vertexList = new List<Vertex>();

                    foreach (var vertex in exteriorRing)
                    {
                        var v = allVertices[vertex];
                        vertexList.Add(v);
                    }
                    var interrings = new List<int>();

                    if (hasInterriorRings)
                    {
                        int count = vertexList.Count;
                        for (var k = 1; k < boundary.Length; k++)
                        {
                            var interiorRing = boundary[k];
                            interrings.Add(count);
                            foreach (var vertex in interiorRing)
                            {
                                var v = allVertices[vertex];
                                vertexList.Add(v);
                                count++;
                            }
                        }
                    }

                    var indices = Tesselate(vertexList, interrings);

                    var triangles = GetTriangles(vertexList, indices);

                    foreach (var triangle in triangles)
                    {
                        var vp0 = new VertexPosition(triangle.A * scale + translate);
                        var vp1 = new VertexPosition(triangle.B * scale + translate);
                        var vp2 = new VertexPosition(triangle.C * scale + translate);

                        prim.AddTriangle(vp0, vp1, vp2);
                    }

                }
            }
        }

        scene.AddRigidMesh(meshBuilder, Matrix4x4.Identity);
        var model = scene.ToGltf2();
        var localTransform = new Matrix4x4(
            1, 0, 0, 0,
            0, 0, -1, 0,
            0, 1, 0, 0,
            0, 0, 0, 1);
        model.ApplyBasisTransform(localTransform);
        var bytes = model.WriteGLB().Array;
        return bytes;
    }

    private static List<Triangle> GetTriangles(List<Vertex> vertices, List<int> indices)
    {
        var triangles = new List<Triangle>();

        for (int k = 0; k < indices.Count; k += 3)
        {
            var v0 = vertices[indices[k]].ToVector3();
            var v1 = vertices[indices[k + 1]].ToVector3();
            var v2 = vertices[indices[k + 2]].ToVector3();

            var triangle = new Triangle(v0, v1, v2);
            triangles.Add(triangle);
        }

        return triangles;
    }

    private static List<int> Tesselate(List<Vertex> p, List<int> interiorRings)
    {
        var normal = GetNormal(p);
        var points2D = Flatten(p, normal);

        var points = new List<double>();
        foreach (var vertex in points2D)
        {
            points.Add(vertex.X);
            points.Add(vertex.Y);
        }

        var result = Earcut.Tessellate(points.ToArray(), interiorRings);
        return result;
    }

    private static List<Vector2> Flatten(List<Vertex> p, Vector3 normal)
    {
        var points = new List<Vector2>();
        if (Math.Abs(normal.X) > Math.Abs(normal.Y) && Math.Abs(normal.X) > Math.Abs(normal.Z))
        {
            //  (yz) projection
            foreach (var vertex in p)
            {
                points.Add(new Vector2((float)vertex.Y, (float)vertex.Z));
            }
        }
        else if (Math.Abs(normal.Y) > Math.Abs(normal.Z))
        {
            foreach (var vertex in p)
            {
                points.Add(new Vector2((float)vertex.X, (float)vertex.Z));
            }
            // # (zx) projection
        }
        else
        {
            // (xy) projextion
            foreach (var vertex in p)
            {
                points.Add(new Vector2((float)vertex.X, (float)vertex.Y));
            }
        }


        return points;
    }
    private static Vector3 GetNormal(List<Vertex> p)
    {
        var v0 = p[0].ToVector3();
        var v1 = p[1].ToVector3();
        var v2 = p[2].ToVector3();
        var u = v1 - v0;
        var v = v2 - v0;
        var normal = Vector3.Cross(u, v);
        normal = Vector3.Normalize(normal);
        return normal;
    }
}
