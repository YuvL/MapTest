using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

            _clusterableItems = LoadPoints();
            _clusterizer = new Clusterizer();
            Update();
        }

        private void Update(double zoom = 1)
        {
            Points.Clear();

            Stopwatch stopwatch = Stopwatch.StartNew();

            var points = _clusterizer.Clusterise<Cluster>(_clusterableItems, zoom);
 

            foreach (var point in points)
            {
                Points.Add(point);
            }

            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds + " мс");
            Console.WriteLine(Points.Count + " т. на карте\r\n");
        }

        private IEnumerable<IPoint> LoadPoints()
        {
            var points = new List<IPoint>();

            for (int i = 0; i < 200; i++)
            {
                double latitude = _random.Next(-90, 90) + _random.NextDouble();
                double longitude = _random.Next(-180, 180) + _random.NextDouble();
                points.Add(new Point(new Location(latitude, longitude)));
            }

            return points;
        }


        private readonly Random _random = new Random();
        private double _zoom;
        private readonly IEnumerable<IPoint> _clusterableItems;
        private Clusterizer _clusterizer;
    }
}