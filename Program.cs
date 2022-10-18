using H3;
using H3.Extensions;
using Microsoft.AspNetCore.Http.Json;
using NetTopologySuite;
using NetTopologySuite.Features;
using OldsaratovMap;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.Converters.Add(new NetTopologySuite.IO.Converters.GeoJsonConverterFactory());
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        b =>
        {
            b
                .AllowAnyOrigin()
                .WithMethods("GET")
                .AllowAnyHeader();
        });
});

var collection = DataImportHelper.ImportData();
var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors();
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("api/map", (double east, double north, double west, double south, int zoom, short from, short to) =>
{
    var bbox = Helpers.CreateBbox(east, north, west, south, geometryFactory);

    var clusters = collection
        .Where(p => p.Location.Within(bbox))
        .Where(x =>
            (!x.PeriodFrom.HasValue && !x.PeriodTo.HasValue) || //records without dates
            (!x.PeriodFrom.HasValue && x.PeriodTo.HasValue && x.PeriodTo.Value > from) || //records without from
            (!x.PeriodTo.HasValue && x.PeriodFrom.HasValue && x.PeriodFrom.Value < to) || //records without to
            (x.PeriodFrom.HasValue && x.PeriodFrom.Value >= from && x.PeriodTo.HasValue && x.PeriodTo.Value <= to))
        .GroupByH3Index(Helpers.GetH3IndexResolutionByZoom(zoom))
        .Select(cluster =>
        {
            var top10 = cluster.Take(5);
            var first = top10.First();
            var count = cluster.Count();

            var rotation = count == 1 ? first.Rotation : null;
            var period = count == 1 ? Helpers.GetPeriodCode(first.PeriodFrom, first.PeriodTo) : byte.MinValue;

            var attributes = new Dictionary<string, object?>
            {
                {AttributeConstants.FeatureAttributeCount, count},
                {AttributeConstants.FeatureAttributeItems, top10},
                {AttributeConstants.FeatureAttributeRotation, rotation},
                {AttributeConstants.FeatureAttributePeriod, period}
            };

            var clusterCenter = count == 1
                ? first.Location
                : new H3Index(cluster.Key).GetCellBoundary(geometryFactory).Centroid;

            var feature = new Feature(clusterCenter, new AttributesTable(attributes));

            return feature;
        }).ToList();

    var featureCollection = new FeatureCollection();
    foreach (var cluster in clusters)
    {
        featureCollection.Add(cluster);
    }

    return featureCollection;
});

app.Run();