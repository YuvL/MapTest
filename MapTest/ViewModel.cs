using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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
                UpdateCluster(_points);
            }
        }

        public ViewModel()
        {



            Points = new ObservableCollection<PointBase>();
            _points = LoadPoints();
              
            UpdateCluster(_points);
        }

        private void UpdateCluster(ObservableCollection<Point> points)
        {
            _xAxisRes = (int) (Math.Pow(10, Zoom*0.3));
            _yAxisRes = (int)(Math.Pow(10, Zoom * 0.3));
            _lonStep = 360.0/_xAxisRes;
            _latStep = 180.0/_yAxisRes;

            ILookup<object, Point> lookup = points.ToLookup(DefineCell);
            Points.Clear();
            foreach (var item in lookup)
            {
                List<Point> list = item.ToList();
                if (list.Count ==1)
                {
                    Points.Add(list.First());
                }
                else
                {
                    Points.Add(new Cluster(list));
                }
            }
        }

        private object DefineCell(Point point)
        {
            double lonNormalized = point.Location.Longitude + 180;
            double latNormalized = point.Location.Latitude + 90;

            var lonCeiling = (int) Math.Ceiling(lonNormalized/_lonStep);
            var latCeiling = (int) Math.Ceiling(latNormalized/_latStep);

            int lonCell = lonCeiling == 0 ? 1 : lonCeiling;
            int latCell = latCeiling == 0 ? 1 : latCeiling;

            var cell = (latCell - 1)*_xAxisRes + lonCell;
            return cell;
        }

        private ObservableCollection<Point> LoadPoints()
        {
            var points = new ObservableCollection<Point>
            {
                new Point {Location = new Location(0, 0)},
                new Point {Location = new Location(0, 180)},
                new Point {Location = new Location(0, -180)},
                new Point {Location = new Location(85, 0)},
                new Point {Location = new Location(-85, 0)},
                new Point {Location = new Location(85, 180)},
                new Point {Location = new Location(-85, 180)},
                new Point {Location = new Location(85, -180)},
                new Point {Location = new Location(-85, -180)}
            };

            for (int i = 0; i < 100; i++)
            {
                int latitude = _random.Next(-10, 10);
                int longitude = _random.Next(-18, 18);
                points.Add(new Point {Location = new Location(latitude, longitude)});
            }

            return points;
        }

        private int _xAxisRes;
        private int _yAxisRes;
        private double _lonStep;
        private double _latStep;
        private readonly ObservableCollection<Point> _points;
        private readonly Random _random = new Random();
        private double _zoom;
    }

    public class Base : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            var propertyChanged = PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class PointBase : Base
    {
        public string Name { get { return string.Format("({0},{1})", Math.Round(location.Latitude), Math.Round(location.Longitude)); } }

        public Location Location
        {
            get { return location; }
            set
            {
                location = value;
                RaisePropertyChanged("Location");
            }
        }

        private string name;
        private Location location;
        private double _width;
    }

    public class Point : PointBase
    {
    }

    public class Cluster : PointBase
    {
        public ObservableCollection<Point> Points { get; set; }

        public Cluster(List<Point> points)
        {
            double lat = points.Select(x => x.Location.Latitude).Sum()/points.Count;
            double lon = points.Select(x => x.Location.Longitude).Sum()/points.Count;

            Points = new ObservableCollection<Point>(points);
            Location = new Location(lat, lon);
        }
    }
}