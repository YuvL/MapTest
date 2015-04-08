using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MapControl;

namespace MapTest
{
    public class ViewModel
    {
        public ObservableCollection<IMapPointBase> Points { get; set; }

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
            Points = new ObservableCollection<IMapPointBase>();

            _clusterableItems = LoadPoints();

            Update();
        }

    

        private void Update(double zoom = 1)
        {
            Points.Clear();

            var points = Clusteriser.Clusterise<Cluster>(_clusterableItems, zoom);

            foreach (var point in points)
            {
                Points.Add(point);
            }
        }


        private IEnumerable<IMapPointBase> LoadPoints()
        {
            var points = new List<IMapPointBase>();

            for (int i = 0; i < 1000; i++)
            {
                double latitude = _random.Next(-90, 90) + _random.NextDouble();
                double longitude = _random.Next(-180, 180) + _random.NextDouble();
                points.Add(new Point {Location = new Location(latitude, longitude)});
            }

            return points;
        }


        private readonly Random _random = new Random();
        private double _zoom;
        private readonly IEnumerable<IMapPointBase> _clusterableItems;
    }
}