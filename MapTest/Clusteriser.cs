using System;
using System.Collections.Generic;
using System.Linq;

namespace MapTest
{
    public interface IMapPointBase
    {
        MapControl.Location Location { get;  }
    }

    public interface ICluster : IMapPointBase
    {
        void SetPoints(IEnumerable<IMapPointBase> nearestItems);
    }

    public interface IMapPoint : IMapPointBase
    {
        double DistanceTo(IMapPointBase point);
    }

    public class Clusteriser
    {
        private const int MarkerSize = 20;

        private static double GetMinAllowableDistance(double zoom)
        {
            //функция подобрана эмпирически
            double scale = Math.Pow(2.0, zoom) / 3.5;
            return MarkerSize / scale;
        }

        public static IEnumerable<IMapPointBase> Clusterise<TCluster>(IEnumerable<IMapPointBase> points, double zoom)
            where TCluster : ICluster, new()
        {
            var minAllowableDistance = GetMinAllowableDistance(zoom);
            var internalPoints = points.Select(x => new InternalContainer(x)).ToList();

            while (true)
            {
                var point = internalPoints.FirstOrDefault(x => !x.IsProcessed && !x.IsCluster);
                if (point != null)
                {
                    List<InternalContainer> nearestPoints = internalPoints.Where(p => !p.IsProcessed && !p.IsCluster && ((IMapPoint)p.Item).DistanceTo(point.Item) < minAllowableDistance).ToList();
                    if (nearestPoints.Count > 1)
                    {
                        foreach (InternalContainer nearesItem in nearestPoints)
                            internalPoints.Remove(nearesItem);

                        var cluster = (ICluster)new TCluster();
                        cluster.SetPoints(nearestPoints.Select(x => x.Item));

                        internalPoints.Add(new InternalContainer(cluster));
                    }
                    else
                    {
                        foreach (var nearbyPoint in nearestPoints)
                            nearbyPoint.IsProcessed = true;
                    }
                }
                else
                {
                    break;
                }
            }
            return internalPoints.Select(x => x.Item);
        }

        private class InternalContainer
        {
            public bool IsProcessed { get; set; }
            public bool IsCluster { get { return Item is ICluster; } }
            public IMapPointBase Item { get { return _clusterable; } }

            public InternalContainer(IMapPointBase item)
            {
                _clusterable = item;
            }

            private readonly IMapPointBase _clusterable;
        }
    }
}