using System;
using System.Collections.Generic;
using System.Linq;

namespace MapTest
{
    /*
        public interface IClusterable
        {
          //  MapControl.Location Location { get; }
            double DistanceTo(IClusterable point);
        }
    */

    public class Clusteriser
    {
        public static IEnumerable<TPoint> Clusterise<TPoint,TCluster>(IEnumerable<TPoint> points, Func<TPoint, TPoint, bool> clusteriseFunc) where TCluster:class 
        {
            var clusterables = points.Select(x => new ClusterableItem<TPoint>(x)).ToList();

            while (true)
            {
                ClusterableItem<TPoint> clusterableItem = clusterables.FirstOrDefault(x => !x.IsProcessed && !x.IsCluster);
                if (clusterableItem != null)
                {
                    var nearestItems = clusterables.Where(p => !clusterableItem.IsProcessed && clusteriseFunc(clusterableItem.Item, p.Item)).ToList();
                    if (nearestItems.Count > 1)
                    {
                        foreach (var nearbyPoint in nearestItems)
                            clusterables.Remove(nearbyPoint);

                        clusterables.Add(new Cluster(nearestItems));
                    }
                    else
                    {
                        foreach (var nearbyPoint in nearestItems)
                            nearbyPoint.IsProcessed = true;
                    }
                }
                else
                {
                    break;
                }
            }
            return clusterables.Cast<TPoint>();
        }
    }

    public class ClusterableItem<T>
    {
        public bool IsProcessed { get; set; }
        public bool IsCluster { get { return _clusterable is ICluster; } }
        public T Item { get { return _clusterable; } }

        public ClusterableItem(T clusterable)
        {
            _clusterable = clusterable;
        }

        private readonly T _clusterable;
    }

    public interface ICluster
    {
    }
}