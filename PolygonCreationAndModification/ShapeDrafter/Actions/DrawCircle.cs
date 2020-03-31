using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using ShapeDrafter.Models;

namespace ShapeDrafter
{
    public partial class MainWindow
    {
        private bool _first;
        private Point _center;
        private List<Circle> _circles = new List<Circle>();
        private void DrawCircle(Point point)
        {
            _first = !_first;
            
            if (_first)
            {
                _center = point;
                return;
            }

            var circle = new Circle(_center, point, _defaultLineColor);
            DrawPoints(circle.Points, _defaultLineColor);
            _circles.Add(circle);
        }
    }
}