using H3;
using H3.Extensions;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using OldsaratovMap.Data;

namespace OldsaratovMap;

public static class GeoJsonFeatureBuilder
{
    public static Feature GetFeatureFromCluster(IGrouping<ulong, Photo> cluster, GeometryFactory geometryFactory)
    {
        var count = cluster.Count();
        if (count == 1)
        {
            return GetFeatureFromPoint(cluster.First());
        }
        
        var top10 = cluster.Take(5);
        
        var attributes = new Dictionary<string, object?>
        {
            {AttributeConstants.FeatureAttributeCount, count},
            {AttributeConstants.FeatureAttributeItems, top10},
            {AttributeConstants.FeatureAttributeKey, cluster.Key}
        };

        var clusterCenter = new H3Index(cluster.Key).GetCellBoundary(geometryFactory).Centroid;

        return new Feature(clusterCenter, new AttributesTable(attributes));
    }

    public static Feature GetFeatureFromPoint(Photo first)
    {
        var rotation = first.Rotation;
        var period = Helpers.GetPeriodCode(first.PeriodFrom, first.PeriodTo);

        var attributes = new Dictionary<string, object?>
        {
            {AttributeConstants.FeatureAttributeCount, 1},
            {AttributeConstants.FeatureAttributeItems, new[] {first}},
            {AttributeConstants.FeatureAttributeRotation, rotation},
            {AttributeConstants.FeatureAttributePeriod, period},
            {AttributeConstants.FeatureAttributeKey, first.Id}
        };

        return new Feature(first.Location, new AttributesTable(attributes));
    }

    public static FeatureCollection GetFeatureCollection(IEnumerable<Feature> clusters)
    {
        var featureCollection = new FeatureCollection();
        foreach (var cluster in clusters)
        {
            featureCollection.Add(cluster);
        }

        return featureCollection;
    }
}