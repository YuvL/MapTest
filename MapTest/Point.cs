using System;
using MapControl;

namespace MapTest
{
    public class Point :  IMapPoint
    {
        public Point(Location location)
        {
            Location = location;
        }

        public string Name { get { return string.Format("({0},{1})", Math.Round(Location.Latitude), Math.Round(Location.Longitude)); } }

        public Location Location { get; private set; }

        public double DistanceTo(IMapPointBase point)
        {
            double x1 = Location.Longitude, x2 = point.Location.Longitude, y1 = Location.Latitude, y2 = point.Location.Latitude;
            return Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
        }
    }
}