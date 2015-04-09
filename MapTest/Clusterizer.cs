using System;
using System.Collections.Generic;
using System.Linq;
using MapControl;

namespace MapTest
{
    public interface IPoint
    {
        Location Location { get; }
    }

    public interface ICluster : IPoint
    {
        void SetPoints(IEnumerable<IPoint> nearestItems);
    }

    public class Clusterizer
    {
        public IEnumerable<IPoint> Clusterise<TCluster>(IEnumerable<IPoint> points, double zoom) where TCluster : ICluster, new()
        {
            var minAllowableDistance = GetMinAllowableDistance(zoom);

            var internalPoints = points.Select(x => new InternalContainer(x)).ToList();

            while (true)
            {
                var point = internalPoints.FirstOrDefault(x => !x.IsProcessed && !x.IsCluster);
                if (point != null)
                {
                    List<InternalContainer> nearestPoints = internalPoints.Where(p => !p.IsProcessed && !p.IsCluster && DistanceBetween(p.Item, point.Item) < minAllowableDistance).ToList();
                    if (nearestPoints.Count > 1)
                    {
                        foreach (InternalContainer nearesItem in nearestPoints)
                            internalPoints.Remove(nearesItem);

                        var cluster = (ICluster) new TCluster();
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

        private double DistanceBetween(IPoint point1, IPoint point2)
        {
            double x1 = point1.Location.Longitude;
            double x2 = point2.Location.Longitude;
            double y1 = point1.Location.Latitude;
            double y2 = point2.Location.Latitude;

            return Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
        }

        private double GetMinAllowableDistance(double zoom)
        {
            //функция подобрана эмпирически
            return 90/ Math.Pow(2.0, zoom);
        }

        private class InternalContainer
        {
            public bool IsProcessed { get; set; }
            public bool IsCluster { get { return Item is ICluster; } }
            public IPoint Item { get { return _item; } }

            public InternalContainer(IPoint item)
            {
                _item = item;
            }

            private readonly IPoint _item;
        }
    }
}