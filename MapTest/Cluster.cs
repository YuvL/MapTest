using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MapControl;

namespace MapTest
{
    public class Cluster : PointBase, ICluster
    {
        public IEnumerable<IClusterable> Points { get; private set; }

        public Cluster(List<IClusterable> points)
        {
            double centroidLat = points.Select(x => x.Location.Latitude).Sum()/points.Count;
            double centroidlon = points.Select(x => x.Location.Longitude).Sum()/points.Count;

            Points = new List<IClusterable>(points);
            Location = new Location(centroidLat, centroidlon);
        }
    }
}