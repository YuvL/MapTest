using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MapControl;

namespace MapTest
{
    public class ViewModel
    {
        public ObservableCollection<PointBase> Points { get; set; }

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
            Points = new ObservableCollection<PointBase>();

            _clusterableItems = LoadPoints();

            Update();
        }

        private static double GetMinAllowableDistance(double zoom)
        {
            //функция подобрана эмпирически
            double scale = Math.Pow(2.0, zoom)/3.5;
            return markerSize/scale;
        }

        private void Update(double zoom = 1)
        {
            Points.Clear();

            var points = Clusteriser.Clusterise(_clusterableItems, ClusteriseFunc);

            foreach (var point in points)
            {
                Points.Add(point);
            }
        }

        private bool ClusteriseFunc(PointBase clusterable, PointBase clusterable1)
        {
            return clusterable.DistanceTo(clusterable1) < GetMinAllowableDistance(Zoom);
        }

        private IEnumerable<PointBase> LoadPoints()
        {
            var points = new List<PointBase>();

            for (int i = 0; i < 1000; i++)
            {
                double latitude = _random.Next(-90, 90) + _random.NextDouble();
                double longitude = _random.Next(-180, 180) + _random.NextDouble();
                points.Add(new Point {Location = new Location(latitude, longitude)});
            }

            return points;
        }

        private const int markerSize = 20;

        private readonly Random _random = new Random();
        private double _zoom;
        private readonly IEnumerable<PointBase> _clusterableItems;
    }
}