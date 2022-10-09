using System.Globalization;
using System.Xml.Serialization;
using CsvHelper;
using CsvHelper.Configuration;
using H3;
using NetTopologySuite.Geometries;
using OldsaratovMap.Data;

namespace OldsaratovMap;

public static class DataImportHelper
{
    public static IReadOnlyCollection<Photo> ImportData()
    {
        var currentFolder = AppContext.BaseDirectory;

        var path = $"{currentFolder}photos.csv";
        
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
        };

        var list = new List<Photo>(25000);

        using var reader = new StreamReader(path);
        using var csv = new CsvReader(reader, config);
        
        csv.Read();
        csv.ReadHeader();
            
        while (csv.Read())
        {
            var photo = new Photo
            {
                Id = csv.GetField<int>("id"),
                Rotation = GetMarkerRotation(csv.GetField("d")!),
                LastUpdate = DateTimeOffset.FromUnixTimeSeconds(csv.GetField<long>("upd")).UtcDateTime,
                Location = new Point(csv.GetField<double>("lon"), csv.GetField<double>("lat")),
                PeriodFrom = csv.GetField<short?>("f"),
                PeriodTo = csv.GetField<short?>("to"),
                PhotoUrl = csv.GetField("p")!,
                Title = csv.GetField("t")!,
                Url = csv.GetField("url")!
            };
                
            photo.H3Index4 = H3Index.FromPoint(photo.Location, 4);
            photo.H3Index5 = H3Index.FromPoint(photo.Location, 5);
            photo.H3Index6 = H3Index.FromPoint(photo.Location, 6);
            photo.H3Index7 = H3Index.FromPoint(photo.Location, 7);
            photo.H3Index8 = H3Index.FromPoint(photo.Location, 8);
            photo.H3Index9 = H3Index.FromPoint(photo.Location, 9);
            photo.H3Index10 = H3Index.FromPoint(photo.Location, 10);
            photo.H3Index11 = H3Index.FromPoint(photo.Location, 11);
            photo.H3Index12 = H3Index.FromPoint(photo.Location, 12);
            photo.H3Index13 = H3Index.FromPoint(photo.Location, 13);
            photo.H3Index14 = H3Index.FromPoint(photo.Location, 14);
                
            list.Add(photo);
        }

        return list;
    }
    
    private static short GetMarkerRotation(string direction)
    {
        return direction switch
        {
            "" => 0,
            "18" => 0,
            "25" => 45,
            "21" => 90,
            "23" => 135,
            "19" => 180,
            "22" => 225,
            "20" => 270,
            "24" => 315,
            _ => throw new ArgumentOutOfRangeException(nameof(direction))
        };
    }
}