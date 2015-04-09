using System;
using MapControl;

namespace MapTest
{
    public class Point : IPoint
    {
        public string Name { get { return string.Format("({0},{1})", Math.Round(Location.Latitude), Math.Round(Location.Longitude)); } }

        public Point(Location location)
        {
            Location = location;
        }

        public Location Location { get; private set; }
    }
}