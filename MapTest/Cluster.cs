using System;
using System.Collections.Generic;
using System.Linq;
using MapControl;

namespace MapTest
{
    public class Cluster : ICluster
    {
        public List<IPoint> Points { get; private set; }
        public string Name { get { return string.Format("({0},{1})", Math.Round(Location.Latitude), Math.Round(Location.Longitude)); } }

        public Location Location { get; private set; }

        public void AddPoints(IEnumerable<IPoint> points)
        {
            var distanceClusterables = points as IPoint[] ?? points.ToArray();
            double centroidLat = distanceClusterables.Select(x => x.Location.Latitude).Sum()/distanceClusterables.Length;
            double centroidlon = distanceClusterables.Select(x => x.Location.Longitude).Sum()/distanceClusterables.Length;

            Points = new List<IPoint>(distanceClusterables);
            Location = new Location(centroidLat, centroidlon);
        }
    }
}