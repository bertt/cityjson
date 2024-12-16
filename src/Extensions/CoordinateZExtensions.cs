using NetTopologySuite.Geometries;

namespace CityJSON.Extensions
{
    public static class CoordinateZExtensions
    {
        public static CoordinateZ Transform(this CoordinateZ coordinate, Transform transform)
        {
            return new CoordinateZ(
                coordinate[0] * transform.Scale[0] + transform.Translate[0],
                coordinate[1] * transform.Scale[1] + transform.Translate[1],
                coordinate[2] * transform.Scale[2] + transform.Translate[2]
            );
        }
    }
}
