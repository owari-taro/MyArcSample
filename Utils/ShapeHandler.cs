using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;

namespace Hello.Utils
{
    public static class ShapeHandler
    {
        /// <summary>
        /// Adds a single point feature to an existing shapefile. The shapefile must be writable.
        /// </summary>
        /// <param name="shapefilePath">Path to the .shp file on disk.</param>
        /// <param name="x">X coordinate (longitude for WGS84).</param>
        /// <param name="y">Y Coordinate (latitude for WGS84).</param>
        /// <param name="attributes">Optional attributes to set on the new feature (key = field name).</param>
        public static async Task AddPointFeatureAsync(string shapefilePath, double x, double y, IDictionary<string, object>? attributes = null)
        {
            if (string.IsNullOrWhiteSpace(shapefilePath))
                throw new ArgumentException("shapefilePath is required", nameof(shapefilePath));

            var table = new ShapefileFeatureTable(shapefilePath);
            Console.WriteLine(table);
            await table.LoadAsync().ConfigureAwait(false);
            
            var feature = table.CreateFeature();

            SpatialReference sr = table.SpatialReference ?? SpatialReferences.Wgs84;
            feature.Geometry = new MapPoint(x, y, sr);

            if (attributes != null)
            {
                foreach (var kv in attributes)
                {
                    if (feature.Attributes.ContainsKey(kv.Key))
                    {
                        Console.WriteLine(kv);
                        feature.Attributes[kv.Key] = kv.Value;
                    }
                }
            }

            await table.AddFeatureAsync(feature).ConfigureAwait(false);
            Console.WriteLine("add feature");
            Console.WriteLine(table.GetField("level").FieldType);

            table.Close();
        }
        public static async Task AddPolygonFeatureAsync(string shapefilePath, Polygon polygon, IDictionary<string, object>? attributes = null)
        {
            if (string.IsNullOrWhiteSpace(shapefilePath))
                throw new ArgumentException("shapefilePath is required", nameof(shapefilePath));

            var table = new ShapefileFeatureTable(shapefilePath);
            Console.WriteLine(table);
            await table.LoadAsync().ConfigureAwait(false);

            var feature = table.CreateFeature();

            //SpatialReference sr = table.SpatialReference ?? SpatialReferences.Wgs84;
            feature.Geometry = polygon;

            if (attributes != null)
            {
                foreach (var kv in attributes)
                {
                    if (feature.Attributes.ContainsKey(kv.Key))
                    {
                        Console.WriteLine(kv);
                        feature.Attributes[kv.Key] = kv.Value;
                    }
                }
            }

            await table.AddFeatureAsync(feature).ConfigureAwait(false);
            Console.WriteLine("add feature");
            Console.WriteLine(table.GetField("level").FieldType);

            table.Close();
        }

        /// <summary>
        /// Adds multiple geometries as a single multipart feature to a shapefile. The shapefile's geometry type
        /// must match the input geometries (all geometries must be of the same compatible type).
        /// Supports Polygon -> multipart Polygon, Polyline -> multipart Polyline, Point -> Multipoint.
        /// </summary>
        public static async Task AddMultiFeatureAsync(string shapefilePath, IEnumerable<Geometry> geometries, IDictionary<string, object>? attributes = null)
        {
            if (string.IsNullOrWhiteSpace(shapefilePath))
                throw new ArgumentException("shapefilePath is required", nameof(shapefilePath));
            if (geometries == null)
                throw new ArgumentNullException(nameof(geometries));

            var geoms = geometries.ToList();
            if (geoms.Count == 0)
                return;
                
            var table = new ShapefileFeatureTable(shapefilePath);
            await table.LoadAsync().ConfigureAwait(false);
            Console.WriteLine(table.GetField("level").FieldType);

            // Clear existing features using extracted helper
            //Console.WriteLine(table.GetField("level").FieldType);

            SpatialReference sr = table.SpatialReference ?? SpatialReferences.Wgs84;

            Geometry multipart;

            switch (table.GeometryType)
            {
                case GeometryType.Polygon:
                    var polyBuilder = new PolygonBuilder(sr);
                    foreach (var g in geoms)
                    {
                        if (g == null) continue;
                        Geometry useGeom = g;
                        if (g.SpatialReference == null || !g.SpatialReference.Equals(sr))
                            useGeom = GeometryEngine.Project(g, sr);
                        var p = useGeom as Polygon;
                        if (p == null) throw new ArgumentException("All supplied geometries must be Polygon instances.", nameof(geometries));
                        foreach (var part in p.Parts)
                            polyBuilder.AddPart(part);
                    }
                    multipart = polyBuilder.ToGeometry();
                    break;

                case GeometryType.Polyline:
                    var lineBuilder = new PolylineBuilder(sr);
                    foreach (var g in geoms)
                    {
                        if (g == null) continue;
                        Geometry useGeom = g;
                        if (g.SpatialReference == null || !g.SpatialReference.Equals(sr))
                            useGeom = GeometryEngine.Project(g, sr);
                        var l = useGeom as Polyline;
                        if (l == null) throw new ArgumentException("All supplied geometries must be Polyline instances.", nameof(geometries));
                        foreach (var part in l.Parts)
                            lineBuilder.AddPart(part);
                    }
                    multipart = lineBuilder.ToGeometry();
                    break;

                default:
                    throw new NotSupportedException($"GeometryType {table.GeometryType} is not supported by AddMultiFeatureAsync.");
            }

            var feature = table.CreateFeature();
            feature.Geometry = multipart;

            if (attributes != null)
            {
                foreach (var kv in attributes)
                {
                    if (feature.Attributes.ContainsKey(kv.Key))
                    {
                        
                        feature.Attributes[kv.Key] = kv.Value;
                    }
                    }
            }

            await table.AddFeatureAsync(feature).ConfigureAwait(false);

            //table=await DeleteAllFeaturesAsync(table).ConfigureAwait(false);
            var query = new QueryParameters { WhereClause = "1=1" };
            var result = await table.QueryFeaturesAsync(query).ConfigureAwait(false);

            // Materialize to a list to avoid modifying the underlying result while iterating
            var features = result.ToList();
            Console.WriteLine(features.Count);

            table.Close();
        }

        // Fix: ensure sr is defined for polyline helper
   

        // New helper: extracts deletion of all features from a table
        private static async Task<ShapefileFeatureTable> DeleteAllFeaturesAsync(ShapefileFeatureTable table)
        {
            if (table == null)
                throw new ArgumentNullException(nameof(table));

            var query = new QueryParameters { WhereClause = "level=0" };
            var result = await table.QueryFeaturesAsync(query).ConfigureAwait(false);

            // Materialize to a list to avoid modifying the underlying result while iterating
            var features = result.ToList();
            Console.WriteLine(features.Count);

          foreach (var feature in features)
                
            {
               // Console.WriteLine("deleting existing feature.");
                await table.DeleteFeatureAsync(feature).ConfigureAwait(false);
            }
            return table;
            //query = new QueryParameters { WhereClause = "1=1" };
         
        }
    }
}

