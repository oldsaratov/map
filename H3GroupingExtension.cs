using OldsaratovMap.Data;

namespace OldsaratovMap;

public static class H3GroupingExtension
{
    public static IEnumerable<IGrouping<ulong, Photo>> GroupByH3Index(this IEnumerable<Photo> source, int resolution)
    {
        return resolution switch
        {
            4 => source.GroupBy(p => p.H3Index4),
            5 => source.GroupBy(p => p.H3Index5),
            6 => source.GroupBy(p => p.H3Index6),
            7 => source.GroupBy(p => p.H3Index7),
            8 => source.GroupBy(p => p.H3Index8),
            9 => source.GroupBy(p => p.H3Index9),
            10 => source.GroupBy(p => p.H3Index10),
            11 => source.GroupBy(p => p.H3Index11),
            12 => source.GroupBy(p => p.H3Index12),
            13 => source.GroupBy(p => p.H3Index13),
            14 => source.GroupBy(p => p.H3Index14),
            
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}