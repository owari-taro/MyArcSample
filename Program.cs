using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.UtilityNetworks;
using Hello.utils;
using Hello.Utils; // 追加

namespace Hello
{
    class Program
    {
        static async Task Main()

        {
            var baseDir = AppContext.BaseDirectory;
            var shpPath = Path.Combine(baseDir, "data", "empty_polygon.shp");
            var sr = SpatialReferences.Wgs84;
            //near tokyo station
            double centerLon = 139.767125;
            double centerLat = 35.681236;

            double offset1 = 0.01; // ~1.1 km
            var points = new PointCollection(sr)
            {
                new MapPoint(centerLon - offset1, centerLat - offset1, sr),
                new MapPoint(centerLon + offset1, centerLat - offset1, sr),
                new MapPoint(centerLon + offset1, centerLat + offset1, sr),
                new MapPoint(centerLon - offset1, centerLat + offset1, sr),
                new MapPoint(centerLon - offset1, centerLat - offset1, sr)
            };
            var poly1 = new Polygon(points);
            //
            var attrs = new Dictionary<string, object>
            {
                ["level"] = (float)3
            };
            await ShapeHandler.AddPolygonFeatureAsync(shpPath, poly1, attrs);

            
        }
        //static async Task ReadShapefileAsync(string shapefilePath)
        //{
        //    if (!File.Exists(shapefilePath))
        //    {
        //        Console.WriteLine($"File not found: {shapefilePath}");
        //        return;
        //    }

        //    var table = new ShapefileFeatureTable(shapefilePath);
        //    await table.LoadAsync().ConfigureAwait(false);

        //    // 全フィーチャを取得（大きいデータならページングや条件付きクエリを使う）
        //    var query = new QueryParameters { WhereClause = "1=1" };
        //    var features = await table.QueryFeaturesAsync(query).ConfigureAwait(false);

        //    foreach (var feature in features)
        //    {
        //        Console.WriteLine(feature);
        //    }

        //}
        // polygon 1 (~1.1km square)
        //double offset1 = 0.01; // ~1.1 km
        //var pc1 = new PointCollection(sr)
        //{
        //    new MapPoint(centerLon - offset1, centerLat - offset1, sr),
        //    new MapPoint(centerLon + offset1, centerLat - offset1, sr), 
        //    new MapPoint(centerLon + offset1, centerLat + offset1, sr),
        //    new MapPoint(centerLon - offset1, centerLat + offset1, sr), 
        //    new MapPoint(centerLon - offset1, centerLat - offset1, sr)
        //};
        //var poly1 = new Polygon(pc1);

        //// polygon 2 (~2.2km square)
        //double offset2 = 0.02; // ~2.2 km
        //var pc2 = new PointCollection(sr)
        //{
        //    new MapPoint(centerLon - offset2, centerLat - offset2, sr),
        //    new MapPoint(centerLon + offset2, centerLat - offset2, sr),
        //    new MapPoint(centerLon + offset2, centerLat + offset2, sr),
        //    new MapPoint(centerLon - offset2, centerLat + offset2, sr),
        //    new MapPoint(centerLon - offset2, centerLat - offset2, sr)
        //};  
        //var poly2 = new Polygon(pc2);

        // attributes: integer field named 'Count' must exist in shapefile schema

        //            await ShapeHandler.AddMultiFeatureAsync(shpPath, new Geometry[] { poly1, poly2 }, attrs);
        // await ShapeHandler.AddMultiFeatureAsync(shpPath, new Geometry[] { poly1,poly2 }, attrs);
        //  Console.WriteLine("Polygons written.");

    }
}