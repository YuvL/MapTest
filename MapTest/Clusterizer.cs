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
        void AddPoints(IEnumerable<IPoint> items);
    }

    public class Clusterizer<TPoint, TCluster> where TPoint : class, IPoint where TCluster : class, ICluster, new()
    {
        public IEnumerable<TPoint> Items
        {
            get
            {
                foreach (var item in _items)
                {
                    yield return item;
                }
            }
        }

        public Clusterizer(Action<IEnumerable<IPoint>> rebuildCallback)
        {
            _rebuildCallback = rebuildCallback;
        }

        public void AddItems(IEnumerable<TPoint> points)
        {
            _items.AddRange(points);
        }

        public void RemoveWhere(Func<TPoint, bool> removePredicat)
        {
            var internalContainers = _items.Where(removePredicat).ToList();
            foreach (var internalContainer in internalContainers)
            {
                _items.Remove(internalContainer);
            }
        }

        public void Rebuild(double zoom)
        {
            IEnumerable<IPoint> clusters = GetClusters(zoom).ToList();
            _rebuildCallback(clusters);
        }

        public void AddItem(TPoint item)
        {
            _items.Add(item);
        }

        private IEnumerable<IPoint> GetClusters(double zoom)
        {
            List<InternalContainer> internalList;

            internalList = _items.Select(item => new InternalContainer(item)).ToList();

            var minAllowableDistance = GetMinAllowableDistance(zoom);

            while (true)
            {
                var point = internalList.FirstOrDefault(x => !x.IsProcessed && !x.IsCluster);
                if (point != null)
                {
                    var nearestPoints = internalList.Where(p => !p.IsProcessed && !p.IsCluster && DistanceBetween(p.Item, point.Item) < minAllowableDistance).ToList();
                    if (nearestPoints.Count > 1)
                    {
                        foreach (var nearesItem in nearestPoints)
                            internalList.Remove(nearesItem);

                        var cluster = (ICluster) new TCluster();
                        cluster.AddPoints(nearestPoints.Select(x => x.Item));

                        internalList.Add(new InternalContainer(cluster, true));
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
            return internalList.Select(x => x.Item);
        }

        private double DistanceBetween(IPoint point1, IPoint point2)
        {
            double x1 = point1.Location.Longitude;
            double x2 = point2.Location.Longitude;
            double y1 = point1.Location.Latitude;
            double y2 = point2.Location.Latitude;

            double distance = Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
            return distance;
        }

        private double GetMinAllowableDistance(double zoom)
        {
            //функция подобрана эмпирически
            return 90/Math.Pow(2.0, zoom);
        }

        private readonly List<TPoint> _items = new List<TPoint>();
        private readonly Action<IEnumerable<IPoint>> _rebuildCallback;

        private class InternalContainer
        {
            public bool IsProcessed { get; set; }
            public bool IsCluster { get; private set; }
            public IPoint Item { get { return _item; } }

            public InternalContainer(IPoint item, bool isCluster = false)
            {
                _item = item;
                IsCluster = isCluster;
            }

            private readonly IPoint _item;
        }
    }
}