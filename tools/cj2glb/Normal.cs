using CityJSON;
using System.Numerics;

namespace cj2glb;
public static class Normal
{
    public static Vector3 GetNormal(List<Vertex> p)
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
