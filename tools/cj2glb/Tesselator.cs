using CityJSON;
using EarcutNet;

namespace cj2glb;
internal class Tesselator
{
    public static List<int> Tesselate(List<Vertex> p, List<int> interiorRings)
    {
        var normal = Normal.GetNormal(p);
        var points2D = Flattener.Flatten(p, normal);

        var points = new List<double>();
        foreach (var vertex in points2D)
        {
            points.Add(vertex.X);
            points.Add(vertex.Y);
        }

        var result = Earcut.Tessellate(points.ToArray(), interiorRings);
        return result;
    }
}
