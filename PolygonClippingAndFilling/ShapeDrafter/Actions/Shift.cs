using System;
using System.Drawing;
using System.Linq;
using System.Windows.Media;
using ShapeDrafter.Drawing;
using ShapeDrafter.Models;

namespace ShapeDrafter
{
    public partial class MainWindow
    {
        private bool _isShifting;
        private Vertex _shiftedVertex;
        private Polygon _shiftedPolygon;
        private Point _lastPoint;
        private WhatIsShifted _what;

        private void ShiftStart(Point point)
        {
            foreach (var polygon in _polygons)
            {
                var vertex = polygon.DetectVertex(point);
                if (vertex != null)
                {
                    _shiftedPolygon = polygon;
                    _shiftedVertex = vertex;
                    _isShifting = true;
                    _what = WhatIsShifted.Vertex;
                    return;
                }
            }
        }
        private void ShapeShiftStart(Point intPoint)
        {
            foreach (var polygon in _polygons)
            {
                foreach (var edge in polygon.Edges)
                {
                    for (int i = 0; i < edge.Points.Count; i++)
                    {
                        var point = edge.Points[i];
                        if (point.Length(intPoint) < 10)
                        {
                            _lastPoint = point;
                            _isShifting = true;
                            _what = WhatIsShifted.Polygon;
                            _shiftedPolygon = polygon;
                            return;
                        }
                    }
                }
            }
        }

        private void ShiftMove(Point point)
        {
            if (_isShifting)
            {
                if (point.X < 0 || point.X> _width || point.Y < 0 || point.Y > _height - 10)
                    return;
                var diff = new Point(point.X - _lastPoint.X, point.Y - _lastPoint.Y);
                switch (_what)
                {
                    case WhatIsShifted.Vertex:
                        _shiftedPolygon.GoodShiftVertex(_shiftedVertex,point);
                        RedrawAll();
                        break;
                    case WhatIsShifted.Polygon:
                        _shiftedPolygon.ShiftAll( diff);
                        break;
                }
            }

            _lastPoint = point;
        }

        private void ShiftEnd()
        {
            _isShifting = false;
        }
    }
}