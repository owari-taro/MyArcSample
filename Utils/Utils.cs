using Esri.ArcGISRuntime.Geometry;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hello.utils
{
    public class Utils
    {
        public static string GetGreeting(string name)
        {
            return $"Hello, {name}!";
        }
    }
    /// <summary>
    /// 
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GeometryCollection<T> where T : Geometry
    {
        private readonly List<T> _items = new();

        public void Add(T geometry)
        {
            _items.Add(geometry);
        }

        public IEnumerable<T> Items => _items;
        public int Count => _items.Count;
       
    }
    
}
