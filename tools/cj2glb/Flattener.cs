using CityJSON;
using System.Numerics;

namespace cj2glb;
public static class Flattener
{
    public static List<Vector2> Flatten(List<Vertex> p, Vector3 normal)
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
            // # (zx) projection
            foreach (var vertex in p)
            {
                points.Add(new Vector2((float)vertex.X, (float)vertex.Z));
            }
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

}
