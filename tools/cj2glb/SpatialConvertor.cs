using System.Numerics;

namespace cj2glb;
public static class SpatialConvertor
{

    public static Vector3 GeodeticToEcef(double lon, double lat, double alt)
    {
        var ellipsoid = new Ellipsoid();
        double equatorialRadius = ellipsoid.SemiMajorAxis;
        double eccentricity = ellipsoid.Eccentricity;
        double latitudeRadians = ToRadius(lat);
        double longitudeRadians = ToRadius(lon);
        double altitudeRadians = alt;
        double num = equatorialRadius / Math.Sqrt(1.0 - Math.Pow(eccentricity, 2.0) * Math.Pow(Math.Sin(latitudeRadians), 2.0));
        double x = (num + altitudeRadians) * Math.Cos(latitudeRadians) * Math.Cos(longitudeRadians);
        double y = (num + altitudeRadians) * Math.Cos(latitudeRadians) * Math.Sin(longitudeRadians);
        double z = ((1.0 - Math.Pow(eccentricity, 2.0)) * num + altitudeRadians) * Math.Sin(latitudeRadians);
        return new Vector3((float)x, (float)y, (float)z);
    }


    public static Matrix4x4 EcefToEnu(Vector3 position)
    {
        var east = new Vector3();
        east.X = -position.Y;
        east.Y = position.X;
        east.Z = 0;

        var eastNormalize = Vector3.Normalize(east);

        var normalUp = GetNormalUp(position);
        var upNormalize = Vector3.Normalize(normalUp);
        var north = Vector3.Cross(normalUp, east);
        var northNormalized = Vector3.Normalize(north);

        return MatrixHelper.GetMatrix(position, eastNormalize, northNormalized, upNormalize);
    }

    private static Vector3 GetNormalUp(Vector3 position)
    {
        var ellipsoid = new Ellipsoid();
        var x = 1.0 / (ellipsoid.SemiMajorAxis * ellipsoid.SemiMajorAxis);
        var y = 1.0 / (ellipsoid.SemiMajorAxis * ellipsoid.SemiMajorAxis);
        var z = 1.0 / (ellipsoid.SemiMinorAxis * ellipsoid.SemiMinorAxis);

        var oneOverRadiiSquared = new Vector3((float)x, (float)y, (float)z);
        var res = Vector3.Multiply(position, oneOverRadiiSquared);
        return Vector3.Normalize(res);
    }


    public static double ToRadius(double degrees)
    {
        double radians = (Math.PI / 180) * degrees;
        return (radians);
    }
}
