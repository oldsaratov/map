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

    var filter = collection
        .Where(p => p.Location.Within(bbox))
        .Where(x =>
            (!x.PeriodFrom.HasValue && !x.PeriodTo.HasValue) || //records without dates
            (!x.PeriodFrom.HasValue && x.PeriodTo.HasValue && x.PeriodTo.Value > from) || //records without from
            (!x.PeriodTo.HasValue && x.PeriodFrom.HasValue && x.PeriodFrom.Value < to) || //records without to
            (x.PeriodFrom.HasValue && x.PeriodFrom.Value >= from && x.PeriodTo.HasValue && x.PeriodTo.Value <= to));

    IEnumerable<Feature> clusters;
    if (zoom < 20) // If zoom is less then max
    {
        clusters = filter
            .GroupByH3Index(Helpers.GetH3IndexResolutionByZoom(zoom))
            .Select(cluster => { return GeoJsonFeatureBuilder.GetFeatureFromCluster(cluster, geometryFactory); });
    }
    else
    {
        clusters = filter.Select(x => GeoJsonFeatureBuilder.GetFeatureFromPoint(x));
    }

    return GeoJsonFeatureBuilder.GetFeatureCollection(clusters);
});

app.Run();