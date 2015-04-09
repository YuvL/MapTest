using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using MapControl;

namespace MapTest
{
    public class ViewModel
    {
        public ObservableCollection<IPoint> Points { get; set; }

        public double Zoom
        {
            get { return _zoom; }
            set
            {
                _zoom = value;
                Update(_zoom);
            }
        }

        public ViewModel()
        {
            Points = new ObservableCollection<IPoint>();

            _clusterizer = new Clusterizer<Point, Cluster>(RebuildCallback);
            _clusterizer.AddItems(LoadPoints<Point>());
            Update();
        }

        private void RebuildCallback(IEnumerable<IPoint> points)
        {
            Points.Clear();
            foreach (var point in points)
            {
                Points.Add(point);
            }
        }

        private void Update(double zoom = 1)
        {
            Points.Clear();

            Stopwatch stopwatch = Stopwatch.StartNew();

            _clusterizer.Rebuild(zoom);

            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds + " мс");
            Console.WriteLine(Points.Count + " т. на карте\r\n");
        }

        private IEnumerable<TPoint> LoadPoints<TPoint>() where TPoint : IPoint
        {
            var points = new List<IPoint>();

            for (int i = 0; i < 50; i++)
            {
                double latitude = _random.Next(-89, 89) + _random.NextDouble();
                double longitude = _random.Next(-179, 179) + _random.NextDouble();
                points.Add(new Point(new Location(latitude, longitude)));
            }

            return points.Cast<TPoint>();
        }

        private readonly Random _random = new Random();
        private double _zoom;
        private readonly Clusterizer<Point, Cluster> _clusterizer;
    }
}