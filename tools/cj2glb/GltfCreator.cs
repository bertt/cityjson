﻿using CityJSON;
using CityJSON.Geometry;
using SharpGLTF.Geometry;
using SharpGLTF.Geometry.VertexTypes;
using SharpGLTF.Materials;
using System.Numerics;

namespace cj2glb;
public static class GltfCreator
{
    public static byte[] ToGltf(CityJsonDocument cityJsonDocument, string id = null)
    {
        var scene = new SharpGLTF.Scenes.SceneBuilder();
        var meshBuilder = new MeshBuilder<VertexPosition, VertexWithFeatureId1, VertexEmpty>("mesh");

        var material1 = new MaterialBuilder()
           .WithDoubleSide(true)
           .WithMetallicRoughnessShader()
           .WithChannelParam(KnownChannel.BaseColor, KnownProperty.RGBA, new Vector4(0.7f, 0, 0f, 1.0f))
           .WithEmissive(new Vector3(0.2f, 0.3f, 0.1f));

        var prim = meshBuilder.UsePrimitive(material1);

        var allVertices = cityJsonDocument.Vertices;
        var appearance = cityJsonDocument.Appearance;
        var transform = cityJsonDocument.Transform;


        var allTriangles = new List<Triangle>();

        var cityObjects = cityJsonDocument.CityObjects;
        if (id != null)
        {
            cityObjects = cityObjects.Where(x => x.Key == id).ToDictionary(x => x.Key, x => x.Value);
        }

        var featureId = 0;
        foreach (var cityObject in cityObjects)
        {
            var co = cityObject.Value;
            ProcessCityObject(co, prim, allVertices, appearance, transform, featureId);
            featureId++;
        }

        scene.AddRigidMesh(meshBuilder, Matrix4x4.Identity);
        var model = scene.ToGltf2();
        AttributesHandler.AddAttributes(cityObjects, model);
        var localTransform = new Matrix4x4(
            1, 0, 0, 0,
            0, 0, -1, 0,
            0, 1, 0, 0,
            0, 0, 0, 1);
        model.ApplyBasisTransform(localTransform);
        var bytes = model.WriteGLB().Array;
        return bytes;
    }

    private static void ProcessCityObject(CityObject co, PrimitiveBuilder<MaterialBuilder, VertexPosition, VertexWithFeatureId1, VertexEmpty> primitiveBuilder, List<Vertex> allVertices, Appearance appearance, Transform transform, int featureId)
    {
        var triangles = GetTriangles(co, allVertices);

        var translate = transform.TranslateVector3();
        var scale = transform.ScaleVector3();

        foreach (var triangle in triangles)
        {
            var vp0 = GetVertexWithFeatureId(triangle.A * scale, featureId);
            var vp1 = GetVertexWithFeatureId(triangle.B * scale, featureId);
            var vp2 = GetVertexWithFeatureId(triangle.C * scale, featureId);

            primitiveBuilder.AddTriangle(vp0, vp1, vp2);
        }
    }

    private static VertexBuilder<VertexPosition, VertexWithFeatureId, VertexEmpty> GetVertexWithFeatureId(Vector3 position, int featureid)
    {
        var vp0 = new VertexPosition(position);
        var v = new VertexWithFeatureId(featureid);
        var vb0 = new VertexBuilder<VertexPosition, VertexWithFeatureId, VertexEmpty>(vp0, v);
        return vb0;
    }

    private static List<Triangle> GetTriangles(CityObject? cityObject, List<Vertex> allVertices)
    {
        var result = new List<Triangle>();

        if(cityObject.Geometry != null)
        {
            var geometries = cityObject!.Geometry;

            foreach (var geometry in geometries)
            {
                if (geometry is CompositeSolidGeometry compositeSolidGeometry)
                {
                    var csTriangles = GetTriangles(compositeSolidGeometry, allVertices);
                    result.AddRange(csTriangles);
                }
                else if (geometry is SolidGeometry solidGeometry)
                {
                    var triangles = GetTriangles(solidGeometry, allVertices);
                    result.AddRange(triangles);
                }
                else if (geometry is MultiSolidGeometry multiSolidGeometry)
                {
                    var csTriangles = GetTriangles(multiSolidGeometry, allVertices);
                    result.AddRange(csTriangles);
                }
                else if (geometry is MultiSurfaceGeometry multiSurfaceGeometry)
                {
                    var csTriangles = GetTriangles(multiSurfaceGeometry, allVertices);
                    result.AddRange(csTriangles);
                }
                else
                {
                    throw new NotImplementedException($"Geometry type {geometry.GetType()} is not implemented.");
                }
            }
        }
        return result;
    }

    private static List<Triangle> GetTriangles(MultiSurfaceGeometry multiSurfaceGeometry, List<Vertex> allVertices)
    {
        var result = new List<Triangle>();
        foreach (var boundary in multiSurfaceGeometry.Boundaries)
        {
            var tris = GetTriangles(boundary, allVertices);
            result.AddRange(tris);
        }
        return result;
    }

    private static List<Triangle> GetTriangles(MultiSolidGeometry multiSolidGeometry, List<Vertex> allVertices)
    {
        var result = new List<Triangle>();
        foreach (var boundary in multiSolidGeometry.Boundaries)
        {
            var tris = GetTriangles(boundary, allVertices);
            result.AddRange(tris);
        }
        return result;
    }

    private static List<Triangle> GetTriangles(CompositeSolidGeometry compositeSolidGeometry, List<Vertex> allVertices)
    {
        var result = new List<Triangle>();
        foreach (var boundary in compositeSolidGeometry.Boundaries)
        {
            var tris = GetTriangles(boundary, allVertices);
            result.AddRange(tris);
        }   
        return result;
    }

    private static List<Triangle> GetTriangles(SolidGeometry solidGeometry, List<Vertex> allVertices)
    {
        var result = new List<Triangle>();
        foreach (var boundary in solidGeometry.Boundaries)
        {
            var triangles = GetTriangles(boundary, allVertices);
            result.AddRange(triangles);
        }
        return result;
    }

    private static List<Triangle> GetTriangles(int[][][][] boundaries, List<Vertex> allVertices)
    {
        var result = new List<Triangle>();
        foreach(var boundary in boundaries)
        {
            var triangles = GetTriangles(boundary, allVertices);
            result.AddRange(triangles);
        }
        return result;
    }

    private static List<Triangle> GetTriangles(int[][][] boundaries, List<Vertex> allVertices)
    {
        var result = new List<Triangle>();
        foreach (var boundary in boundaries)
        {
            var triangles = GetTriangles(boundary, allVertices);
            result.AddRange(triangles);
        }
        return result;
    }


    private static List<Triangle> GetTriangles(int[][] boundaries, List<Vertex> allVertices)
    {
        var hasInterriorRings = boundaries.Length > 1;
        var exteriorRing = boundaries[0];

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
            for (var k = 1; k < boundaries.Length; k++)
            {
                var interiorRing = boundaries[k];
                interrings.Add(count);
                foreach (var vertex in interiorRing)
                {
                    var v = allVertices[vertex];
                    vertexList.Add(v);
                    count++;
                }
            }
        }

        var indices = Tesselator.Tesselate(vertexList, interrings);

        var triangles = GetTriangles(vertexList, indices);
        return triangles;
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
}
