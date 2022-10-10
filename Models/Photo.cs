using System.Text.Json.Serialization;
using NetTopologySuite.Geometries;

namespace OldsaratovMap.Data;

public class Photo
{
    [JsonIgnore] public int Id { get; set; }
    public string Title { get; set; }
    public string PhotoUrl { get; set; }
    public short? PeriodFrom { get; set; }
    public short? PeriodTo { get; set; }
    [JsonIgnore] public short? Rotation { get; set; }
    public string Url { get; set; }
    [JsonIgnore] public Point Location { get; set; }
    [JsonIgnore] public DateTime LastUpdate { get; set; }
    [JsonIgnore] public ulong H3Index4 { get; set; }
    [JsonIgnore] public ulong H3Index5 { get; set; }
    [JsonIgnore] public ulong H3Index6 { get; set; }
    [JsonIgnore] public ulong H3Index7 { get; set; }
    [JsonIgnore] public ulong H3Index8 { get; set; }
    [JsonIgnore] public ulong H3Index9 { get; set; }
    [JsonIgnore] public ulong H3Index10 { get; set; }
    [JsonIgnore] public ulong H3Index11 { get; set; }
    [JsonIgnore] public ulong H3Index12 { get; set; }
    [JsonIgnore] public ulong H3Index13 { get; set; }
    [JsonIgnore] public ulong H3Index14 { get; set; }
}