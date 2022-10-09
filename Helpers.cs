using NetTopologySuite.Geometries;

namespace OldsaratovMap;

internal class Helpers
{
    internal static Polygon CreateBbox(double east, double north, double west, double south, GeometryFactory factory)
    {
        return factory.CreatePolygon(new[]
        {
            new Coordinate(east, north),
            new Coordinate(west, north),
            new Coordinate(west, south),
            new Coordinate(east, south),
            new Coordinate(east, north)
        });
    }

    internal static byte GetPeriodCode(short? periodFrom, short? periodTo)
    {
        if (!periodFrom.HasValue || !periodTo.HasValue) return 0;

        var middle = (periodFrom + periodTo) / 2;

        return middle switch
        {
            < 1900 => 1,
            >= 1900 and <= 1920 => 2,
            >= 1921 and <= 1930 => 3,
            >= 1931 and <= 1940 => 4,
            >= 1941 and <= 1950 => 5,
            >= 1951 and <= 1960 => 6,
            >= 1961 and <= 1970 => 7,
            >= 1971 and <= 1980 => 8,
            >= 1981 and <= 1990 => 9,
            >= 1991 => 10,
        };
    }

    internal static int GetH3IndexResolutionByZoom(int zoom)
    {
        return zoom switch
        {
            10 => 4,
            11 => 5,
            12 => 6,
            13 => 7,
            14 => 8,
            15 => 9,
            16 => 10,
            17 => 11,
            18 => 12,
            19 => 13,
            20 => 14,
            _ => throw new ArgumentOutOfRangeException(nameof(zoom))
        };
    }
}