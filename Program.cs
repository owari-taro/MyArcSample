using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Security;
using Esri.ArcGISRuntime.UtilityNetworks;
using Hello.utils;
using Hello.Utils; // 追加

using Esri.ArcGISRuntime;
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
        }
 

}