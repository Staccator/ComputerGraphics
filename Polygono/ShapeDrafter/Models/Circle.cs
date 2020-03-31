using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using static ShapeDrafter.Drawing.DrawingExtensions;

namespace ShapeDrafter.Models
{
    public class Circle
    {
        private Point _center;
        private readonly Color _color;
        private readonly MainWindow _window;
        private Point _point;

        public Circle(Point center, Point point, Color defaultLineColor)
        {
            _point = point;
//            _center = new Point((center.X + point.X) / 2, (center.Y + point.Y) / 2);
            _center = center;
            _window = MainWindow.instance;
            var points = GetCircle(_center.X, _center.Y, point.X, point.Y, _window._width,
                _window._height);
            Points = points;
            _color = defaultLineColor;
        }

        public List<Point> Points { get; private set; }

        public void ShiftAll(Point diff)
        {
            ClearCircle();
            _point = _point.Add(diff);
            _center = _center.Add(diff);
            Points = GetCircle(_center.X , _center.Y , _point.X, _point.Y, _window._width, _window._height);
//            Points = Points.Select(p => p.Add(diff)).ToList();
            _window.RedrawAll();
        }

        private void ClearCircle()
        {
            _window.DrawPoints(Points, Color.White);
        }

        public void Redraw(bool clear = false)
        {
            if (clear)
            {
                _window.DrawPoints(Points, Color.White);
            }
            else
            {
                _window.DrawPoints(Points, _color);
            }
        }

        public void ShiftRadius(Point point)
        {
            ClearCircle();
            Points = GetCircle(_center.X, _center.Y, point.X, point.Y, _window._width, _window._height);
            _point = point;
            _window.RedrawAll();
        }
    }
}