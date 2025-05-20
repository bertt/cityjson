namespace cj2glb;

using DotSpatial.Projections;
using NetTopologySuite.Geometries;
using System;

public static class CoordinateTransformer
{
    public static Coordinate TransformToWGS84(double x, double y, double altitude, string sourceCrsUrn)
    {
        var epsgCode = ExtractEpsgCode(sourceCrsUrn);
        if (epsgCode != null)
        {
            double[] xy = [ x, y ];
            double[] z = [altitude];

            ProjectionInfo fromProjection = null;
            
        // todo: support other epsg codes
            switch (epsgCode)
            {
                case 28992: 
                    fromProjection = KnownCoordinateSystems.Projected.NationalGrids.Rijksdriehoekstelsel;
                    break;
                default:
                    throw new NotSupportedException($"EPSG code {epsgCode} is not supported.");
            }
            ProjectionInfo toWGS84 = KnownCoordinateSystems.Geographic.World.WGS1984;

            Reproject.ReprojectPoints(xy, z, fromProjection, toWGS84, 0, 1);
            return new Coordinate(xy[0], xy[1]);
        }

        return null;
    }

    private static int? ExtractEpsgCode(string urn)
    {
        if (string.IsNullOrWhiteSpace(urn))
            throw new ArgumentException("URN is null or empty.");

        var parts = urn.Split(':');
        if (parts.Length < 1 || !int.TryParse(parts[^1], out int epsg))
            throw new FormatException("URN does not end with a valid EPSG code.");

        return epsg;
    }
}