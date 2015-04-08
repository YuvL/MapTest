using System;
using MapControl;

namespace MapTest
{
    public class PointBase : Base
    {
        public string Name { get { return string.Format("({0},{1})", Math.Round(location.Latitude), Math.Round(location.Longitude)); } }

        public Location Location
        {
            get { return location; }
            set
            {
                location = value;
                RaisePropertyChanged("Location");
            }
        }

        public bool IsProcessed { get; set; }

        public double DistanceTo(PointBase point)
        {
            double x1 = Location.Longitude, x2 = point.Location.Longitude, y1 = Location.Latitude, y2 = point.Location.Latitude;
            return Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
        }

        private Location location;
    }
}